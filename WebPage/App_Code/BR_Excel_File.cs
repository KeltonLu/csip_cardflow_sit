/// <summary>
/// 修改時間:2020/11/04_Ares_Stanley-移除Micriosoft Excel, 新增NPOI
/// 修改時間:2021/01/20_Joe-RQ-2019-008159-003：配合長姓名作業修改
/// </summary>
using CSIPCommonModel.BusinessRules;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using DataTable = System.Data.DataTable;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;

/// <summary>
/// BR_Excel_File 的摘要描述
/// 修改日期: 2020/11/06_Ares_Stanley-變更0510資料順序; 2020/11/17_Ares_Stanley-變更0519資料順序
///           2020/11/23_Ares_Luke   -變更檢查目錄機制;
/// </summary>
public class BR_Excel_File : BRBase<Entity_UnableCard>
{
    #region 各報表SQL語句

    #region SearchExport0502
    //20210120 (U)by Joe, 增加羅馬拼音
    private const string SearchExport0502 = @"
SELECT u.ImportDate,
       CASE
           b.Merch_Code
           WHEN 'A' THEN
               '宏通'
           WHEN 'B' THEN
               '台銘'
           WHEN 'C' THEN
               '金雅拓'
           END AS Merch_NAME,
       u.indate1,
       u.id,
       u.CustName,
       u.CustName_Roma,
       u.CardNo,
       u.blockcode,
       CASE
           u.OutputFlg
           WHEN 'N' THEN
               '未處理'
           WHEN 'Y' THEN
               '已處理'
           WHEN 'T' THEN
               '退單'
           WHEN 'S' THEN
               '成功'
           ELSE ''
           END AS outPutFlg
FROM
	tbl_UnableCard u
	INNER JOIN tbl_Card_BaseInfo AS b ON u.indate1= b.indate1 
	AND u.action = b.action 
	AND u.CardNo= b.CardNo 
WHERE ( ImportDate BETWEEN @startDate AND @endDate ) 
 	AND ( @outFlg = '00' OR OutputFlg = @outFlg ) 
 	AND ( @factory = '00' OR b.Merch_Code = @factory ) 
ORDER BY
	u.Indate1";

    #endregion

    #region SearchExport0503

    private const string SearchExport0503 = @"
SELECT
	ImportDate,
	CardNo,
    BlockCode,
CASE
		OutputFlg 
		WHEN 'N' THEN
		'未處理' 
		WHEN 'Y' THEN
		'已處理' 
		WHEN 'T' THEN
		'退單' 
		WHEN 'S' THEN
		'成功' ELSE '' 
	END AS outPutFlg 
FROM
	tbl_CardChange 
WHERE
	( ImportDate BETWEEN @startDate AND @endDate ) 
	AND ( @outFlg = 'NULL' ) 
	OR ( ImportDate BETWEEN @startDate AND @endDate ) 
	AND ( OutputFlg = @outFlg )
";

    #endregion

    #region SearchExport0504

    private const string SearchExport0504 = @"
SELECT
	serial_no,
CASE
		Kind 
		WHEN '1' THEN
		'卡片' 
		WHEN '2' THEN
		'ATM' 
		WHEN '4' THEN
		'現金卡' 
		WHEN '5' THEN
		'VISA DEBIT' 
		WHEN '6' THEN
		'GIFT CARD-一般' 
		WHEN '7' THEN
		'GIFT CARD-大包' 
		WHEN '8' THEN
		'GIFT CARD-小包' ELSE '' 
	END AS Kind,
CASE
	ACTION 
		WHEN '1' THEN
		'新卡' 
		WHEN '2' THEN
		'掛失補發卡' 
		WHEN '3' THEN
		'毀損補發卡' 
		WHEN '4' THEN
		'補發密碼' 
		WHEN '5' THEN
		'年度換卡' ELSE '' 
	END AS ACTION,
	Backdate,
CASE
		Reason 
		WHEN '1' THEN
		'招領逾期' 
		WHEN '2' THEN
		'無此人' 
		WHEN '3' THEN
		'址欠詳' 
		WHEN '4' THEN
		'遷移不明' 
		WHEN '5' THEN
		'拒收' 
		WHEN '6' THEN
		'離職' 
		WHEN '7' THEN
		'死亡' 
		WHEN '8' THEN
		'信箱退租' 
		WHEN '9' THEN
		'原因不明' ELSE '' 
	END AS Reason,
	Cardno,
	Closedate,
CASE
		Enditem 
		WHEN '0' THEN
		'自取' 
		WHEN '1' THEN
		'普掛' 
		WHEN '2' THEN
		'限掛' 
		WHEN '3' THEN
		'快遞' 
		WHEN '4' THEN
		'夜間投遞' 
		WHEN '5' THEN
		'註銷' 
		WHEN '6' THEN
		'碎卡' ELSE '' 
	END AS Enditem,
	Maildate,
	Mailno,
	Enduid 
FROM
	tbl_Card_BackInfo 
WHERE
	( @BackdateStart = 'NULL' OR Backdate >=@BackdateStart ) 
	AND ( @BackdateEnd = 'NULL' OR Backdate <=@BackdateEnd ) 
	AND ( @ClosedateStart = 'NULL' OR Closedate >=@ClosedateStart ) 
	AND ( @ClosedateEnd = 'NULL' OR Closedate <=@ClosedateEnd ) 
	AND ( @serial_no = 'NULL' OR serial_no =@serial_no ) 
	AND ( @Id = 'NULL' OR Id =@Id ) 
	AND ( @Cardno = 'NULL' OR Cardno =@Cardno )
";

    #endregion

    #region SearchExport0506

    private const string SearchExport0506 = @"
SELECT
	CardNo,
	BlockCode,
	Memo,
	MemoLog,
	SFFlg 
FROM
	tbl_CancelOASA_Detail 
WHERE 
 	( @processDateStart = 'NULL' OR CancelOASADate >=@processDateStart ) 
 	AND ( @processDateEnd = 'NULL' OR CancelOASADate <=@processDateEnd ) 
    AND ( @status = 'NULL' OR SFFlg = @status ) 
 	AND ( BlockCode =@blockCode ) 
 	AND ( Memo =@memo )
";

    #endregion

    #region SearchExport0507

    private const string SearchExport0507 = @"
SELECT ID,
       CardNo,
       Imp_Date,
       Maildate,
       Mailno,
       Ams
FROM tbl_sletterInfo_CallCust
WHERE (@ID ='NULL' OR ID=@ID)
AND   (@ProcessDateStart ='NULL' OR Exp_Date>=@ProcessDateStart)
AND   (@ProcessDateEnd ='NULL' OR Exp_Date<=@ProcessDateEnd)
AND   (@MaildateStart ='NULL' OR Maildate<=@MaildateStart)
AND   (@MaildateEnd ='NULL' OR Maildate<=@MaildateEnd)
AND   (@Mailno ='NULL' OR Mailno=@Mailno)
AND   (@Status ='NULL' OR Ams=@Status)
";

    #endregion

    #region SearchExport0508

    private const string SearchExport0508 = @"
SELECT B.ID
      ,B.CardNo
      ,S.Imp_Date
      ,S.Maildate
      ,S.Mailno
      ,case S.Info1 when '316' then '投遞不成功' when '223' then '投遞成功' when '240' then '招領中' when '247' then '註銷投遞成功記錄' when '258' then '退件' end as Info1Name
  FROM tbl_Post_Send S left join tbl_Card_BaseInfo B ON S.Maildate=B.Maildate AND 
S.Mailno=B.Mailno
WHERE (@ID ='NULL' OR B.ID=@ID)
AND   (@Imp_DateStart ='NULL' OR S.Imp_Date>=@Imp_DateStart)
AND   (@Imp_DateEnd ='NULL' OR S.Imp_Date<=@Imp_DateEnd)
AND   (@MaildateStart ='NULL' OR S.Maildate>=@MaildateStart)
AND   (@MaildateEnd ='NULL' OR S.Maildate<=@MaildateEnd)
AND   (@Mailno ='NULL' OR S.Mailno=@Mailno)
AND   (@Non_Send_Code ='NULL' OR S.Info1=@Non_Send_Code)
";

    #endregion

    #region SearchExport0509

    private const string SearchExport0509 = @"
select maildate,
       cardtype,
       sum(allnum) as allnum,
       mailno
from (SELECt distinct maildate,
                      isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype), '信用卡') as cardtype,
                      count(*)                                                                              as allnum,
                      substring(dbo.f_AppendMailno(maildate, cardtype, @branchid), 1, 6)                    as mailno
      FROM dbo.tbl_Card_BaseInfo base
      where (@maildatefrom = '' or convert(datetime, maildate, 120) >= convert(datetime, @maildatefrom, 120))
        and (@maildateto = '' or convert(datetime, maildate, 120) <= convert(datetime, @maildateto, 120))
        and (@indatefrom = '' or convert(datetime, indate1, 120) >= convert(datetime, @indatefrom, 120))
        and (@indateto = '' or convert(datetime, indate1, 120) <= convert(datetime, @indateto, 120))
        and branch_id = @branchid
        --and kind='23'
        and kind in ('21', '22', '23', '24')
      GROUP BY maildate, cardtype) t
GROUP BY mailno, maildate, cardtype
ORDER BY maildate DESC
";

    #endregion

    #region SearchExport0510
    //20210120 (U)by Joe, 增加羅馬拼音
    private const string SearchExport0510 = @"
select id,custname,custname_roma,cardno,kktime, indate1,UpdDate,CNote 
from tbl_HoldCard 
order by convert(int,kktime) DESC
";

    #endregion

    #region SearchExport0519

    private const string SearchExport0519 = @"
select a.id,
       a.cardno,
       a.indate1,
       b.Backdate,
       a.action,
       (case
            when a.Action = '1' then '新卡'
            when a.Action = '2' then '掛失補發卡'
            when a.Action = '3' then '毀損補發卡'
            when a.Action = '4' then '補發密碼'
            when a.Action = '5'
                then '年度換卡'
            else '' end
           )                as ActionName,
       b.Closedate,
       ''                   as CloseCode,
       ''                   as CloseReason,
       a.maildate,
       b.Reason,
       (
           case
               when b.Reason = '1' then '招領逾期'
               when b.Reason = '2' then '無此人'
               when b.Reason = '3' then '址欠詳'
               when b.Reason = '4' then '遷移不明'
               when b.Reason = '5' then '拒收'
               when b.Reason = '6' then '離職'
               when b.Reason = '7' then '死亡'
               when b.Reason = '8' then '信箱退租'
               when b.Reason = '9' then '原因不明'
               else '' end
           )                as ReasonName,
       a.cardtype,
       a.affinity,
       a.photo,
       (
           case
               when a.Merch_Code = 'A' then '宏通'
               when a.Merch_Code = 'B' then '台銘'
               when a.Merch_Code = 'C' then '金雅拓'
               else '' end
           )                as Factory,
       a.expdate,
       a.kind,
       (
           case
               when a.kind = '0' then '普掛'
               when a.kind = '1' then '自取'
               when a.kind = '2' then '卡交介'
               when a.kind = '3' then '限掛'
               when a.kind = '4' then '快遞'
               when a.kind = '5' then '三天快速發卡'
               when a.kind = '6' then '保留'
               when a.kind = '7' then '其他'
               when a.kind = '8' then '包裹'
               when a.kind = '9' then '無法製卡'
               when a.kind = '10' then '卡片碎卡'
               when a.kind = '11' then '卡片註銷'
               when a.kind = '21' then '預製卡-無帳號'
               when a.kind = '22' then '預製卡-有帳號'
               when a.kind = '23' then '郵寄分行'
               when a.kind = '24' then '整批撥薪'
               when a.kind = '25' then 'RNMAIL'
               else '' end
           )                as kindName,
       a.zip,
       (add1 + add2 + add3) as address,
       branch_id,
       seq
from tbl_Card_BaseInfo a WITH (nolock)
         left join tbl_Card_BackInfo b WITH (nolock) on
        a.action = b.action and a.id = b.id and a.cardno = b.cardno and a.trandate = b.trandate
where (@backdate = 'NULL' or b.backdate in (select max(b.Backdate)
                                            from tbl_Card_BaseInfo a
                                                     join tbl_Card_BackInfo b on
                                                    a.action = b.action and a.id = b.id and a.cardno = b.cardno and
                                                    a.trandate = b.trandate
                                            group by a.id, a.cardno, a.action, a.trandate))
  and (@indatefrom = 'NULL' or a.indate1 between @indatefrom and @indateto)
  and (@bdatefrom = 'NULL' or b.Backdate between @bdatefrom and @bdateto)
  and (@actionstart = '00' or @actionend = '00' or
       (convert(int, a.Action) between convert(int, @actionstart) and convert(int, @actionend)))
  and (@cardtypestart = '00' or @cardtypeend = '00' or
       (convert(int, a.cardtype) between convert(int, @cardtypestart) and convert(int, @cardtypeend)))
  and (@affinitystart = '00' or @affinityend = '00' or
       (convert(int, a.affinity) between convert(int, @affinitystart) and convert(int, @affinityend)))
  and (@kindstart = '00' or @kindend = '00' or
       (convert(int, a.kind) between convert(int, @kindstart) and convert(int, @kindend)))
  and (@photostart = '00' or @photoend = '00' or
       (convert(int, a.photo) between convert(int, @photostart) and convert(int, @photoend)))
  and (@factory = '00' or a.Merch_Code = @factory)
  and (@mailDateFrom = 'NULL' or a.maildate between @mailDateFrom and @mailDateTo)
  and (@mailNo = 'NULL' or a.mailno = @mailNo)
";

    #endregion

    #region SearchExport0516
    /// <summary>
    /// 修改紀錄：20210618_Ares_Stanley-變更資料排序
    /// END0：自取/END1：普掛/END2：限掛/END3：快遞/END4：夜間投遞/END5：卡片註銷/END6：卡片碎卡/END7：其他
    /// </summary>
    private const string SearchExport0516 = @"
select ReasonName, sum(DayIn) DayIn,sum(DayOut) DayOut,sum(CountEnd) CountEnd
,sum(END1) END1,sum(END2) END2,sum(END6) END6,sum(END4) END4,sum(END0) END0,sum(END3) END3,sum(END5) END5,sum(END7) END7
from (
select T.cardtype,
     isnull((select CardTypeName From tbl_CardType where CardType=T.cardtype),'信用卡') ReasonName,
     sum(T.DayIn) as DayIn,sum(T.DayOut) as DayOut,sum(T.CountEnd) as CountEnd,
     sum(T.END0) as END0,sum(T.END1) as END1,
     sum(T.END2) as END2,sum(T.END3) as END3,
     sum(T.END4) as END4,sum(T.END5) as END5,
     sum(T.END6) as END6,sum(T.END7) as END7
from
( 
     select isnull((select CardType From tbl_CardType where CardType=n.cardtype),'999') cardtype,
         n.DayIn,n.DayOut,(n.CountEndNow+n.CountEndBefore-n.CountInBefore) as CountEnd,n.END0,
         n.END1,n.END2,n.END3,n.END4,n.END5,n.END6,n.END7
     from
     (    select distinct m.cardtype,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and ImportDate=@Operaction ) as DayIn,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Closedate=@Operaction ) as DayOut,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and isnull(CardBackStatus,'') = '2' ) as CountEndNow,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and  isnull(Closedate,'')= @Operaction ) as CountEndBefore,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and  isnull(ImportDate,'')= @Operaction ) as CountInBefore,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='0' and Closedate=@Operaction ) as END0,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='1' and Closedate=@Operaction ) as END1,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='2' and Closedate=@Operaction ) as END2,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='3' and Closedate=@Operaction ) as END3,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='4' and Closedate=@Operaction ) as END4,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='5' and Closedate=@Operaction ) as END5,
              (select count(Serial_no) from tbl_Card_BackInfo where cardtype=m.cardtype and Enditem='6' and Closedate=@Operaction ) as END6,
              (select count(Serial_no) from tbl_Card_BackInfo  where cardtype=m.cardtype and Enditem='7' and Closedate=@Operaction ) as END7
         from 
         (
              select Enditem,cardtype,ImportDate,Enddate,CardBackStatus
              from tbl_Card_BackInfo 
              group by cardtype,Enditem,ImportDate,Enddate,CardBackStatus
         ) m
     )n
) T
group by cardtype
) z
group by ReasonName
";

    #endregion

    #region SearchExport0517

    private const string SearchExport0517 = @"
select distinct -- m.Reason,
                CASE m.Reason
                    WHEN '1' THEN '招領逾期'
                    WHEN '2' THEN '無此人'
                    WHEN '3' THEN '址欠詳'
                    WHEN '4' THEN '遷移不明'
                    WHEN '5' THEN '拒收'
                    WHEN '6' THEN '離職'
                    WHEN '7' THEN '死亡'
                    WHEN '8' THEN '信箱退租'
                    WHEN '9' THEN '原因不明'
                    END                                            as ReasonName,
                (select count(cardno)
                 from tbl_Card_BackInfo
                 where Reason = m.Reason
                   and Action = '1'
                   and (Action = @Action OR @Action = 'NULL')
                   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action1,
                (select count(cardno)
                 from tbl_Card_BackInfo
                 where Reason = m.Reason
                   and Action = '2'
                   and (Action = @Action OR @Action = 'NULL')
                   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action2,
                (select count(cardno)
                 from tbl_Card_BackInfo
                 where Reason = m.Reason
                   and Action = '3'
                   and (Action = @Action OR @Action = 'NULL')
                   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action3,
                (select count(cardno)
                 from tbl_Card_BackInfo
                 where Reason = m.Reason
                   and Action = '4'
                   and (Action = @Action OR @Action = 'NULL')
                   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action4,
                (select count(cardno)
                 from tbl_Card_BackInfo
                 where Reason = m.Reason
                   and Action = '5'
                   and (Action = @Action OR @Action = 'NULL')
                   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action5,
                (select count(cardno)
                 from tbl_Card_BackInfo
                 where Reason = m.Reason
                   and Action in ('1','2','3','4','5')
                   and (Action = @Action OR @Action = 'NULL')
                   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action6
from (
         select Reason
         from tbl_Card_BackInfo
         where (Backdate BETWEEN @MstatrDate AND @MendDate)
           AND (Action = @Action OR @Action = 'NULL')
         group by Reason
     ) m
";

    #endregion

    #region SearchExport0518

    private const string SearchExport0518 = @"
if @backtype = 1 --本日新增
    begin
        select a.serial_no,
               a.CustName,
               a.id,
               isnull((Select CardTypeName From tbl_CardType where CardType = a.cardtype), '信用卡') cardtype,
               (case
                    when a.Action = '1' then '新卡'
                    when a.Action = '2' then '掛失補發卡'
                    when a.Action = '3' then '毀損補發卡'
                    when a.Action = '4' then '補發密碼'
                    when a.Action = '5' then '年度換卡'
                    else '' end) as                                                               ActionName,
               --a.Action,
               a.CardNo,
               a.Backdate,
               (case
                    when a.Reason = '1' then '招領逾期'
                    when a.Reason = '2' then '無此人'
                    when a.Reason = '3' then '址欠詳'
                    when a.Reason = '4' then '遷移不明'
                    when a.Reason = '5' then '拒收'
                    when a.Reason = '6' then '離職'
                    when a.Reason = '7' then '死亡'
                    when a.Reason = '8' then '信箱退租'
                    when a.Reason = '9' then '原因不明'
                    else '' end) as                                                               Reason,
               a.Closedate,
               (case
                    when a.Enditem = '0' then '自取'
                    when a.Enditem = '1' then '普掛'
                    when a.Enditem = '2' then '限掛'
                    when a.Enditem = '3' then '快遞'
                    when a.Enditem = '4' then '夜間投遞'
                    when a.Enditem = '5' then '註銷'
                    when a.Enditem = '6' then '碎卡'
                    else '' end) as                                                               Enditem
        from tbl_Card_BackInfo a
                 join tbl_Card_BaseInfo b
                      on a.cardno = b.cardno and a.action = b.action and a.id = b.id and a.trandate = b.trandate
        where --a.CardBackStatus <> '2'
          --and
            (@datefrom is null or convert(datetime, a.ImportDate, 120) >= convert(datetime, @datefrom, 120))
          and (@dateto is null or convert(datetime, a.ImportDate, 120) <= convert(datetime, @dateto, 120))
        order by a.Enddate, a.Action, a.serial_no
    end
else
    if @backtype = 2 --本日結案
        begin
            select a.serial_no,
                   a.CustName,
                   a.id,
                   isnull((Select CardTypeName From tbl_CardType where CardType = a.cardtype), '信用卡') cardtype,
                   (case
                        when a.Action = '1' then '新卡'
                        when a.Action = '2' then '掛失補發卡'
                        when a.Action = '3' then '毀損補發卡'
                        when a.Action = '4' then '補發密碼'
                        when a.Action = '5' then '年度換卡'
                        else '' end) as                                                               ActionName,
                   --a.Action,
                   a.CardNo,
                   a.Backdate,
                   (case
                        when a.Reason = '1' then '招領逾期'
                        when a.Reason = '2' then '無此人'
                        when a.Reason = '3' then '址欠詳'
                        when a.Reason = '4' then '遷移不明'
                        when a.Reason = '5' then '拒收'
                        when a.Reason = '6' then '離職'
                        when a.Reason = '7' then '死亡'
                        when a.Reason = '8' then '信箱退租'
                        when a.Reason = '9' then '原因不明'
                        else '' end) as                                                               Reason,
                   a.Closedate,
                   (case
                        when a.Enditem = '0' then '自取'
                        when a.Enditem = '1' then '普掛'
                        when a.Enditem = '2' then '限掛'
                        when a.Enditem = '3' then '快遞'
                        when a.Enditem = '4' then '夜間投遞'
                        when a.Enditem = '5' then '註銷'
                        when a.Enditem = '6' then '碎卡'
                        else '' end) as                                                               Enditem
            from tbl_Card_BackInfo a
                     join tbl_Card_BaseInfo b
                          on a.cardno = b.cardno and a.action = b.action and a.id = b.id and a.trandate = b.trandate
            where a.CardBackStatus = '2'
              and (@datefrom is null or convert(datetime, a.Closedate, 120) >= convert(datetime, @datefrom, 120))
              and (@dateto is null or convert(datetime, a.Closedate, 120) <= convert(datetime, @dateto, 120))
            order by a.Enddate, a.Action, a.serial_no
        end
    else
        if @backtype = 3 --未結案
            begin
                select a.serial_no,
                       a.CustName,
                       a.id,
                       isnull((Select CardTypeName From tbl_CardType where CardType = a.cardtype), '信用卡') cardtype,
                       (case
                            when a.Action = '1' then '新卡'
                            when a.Action = '2' then '掛失補發卡'
                            when a.Action = '3' then '毀損補發卡'
                            when a.Action = '4' then '補發密碼'
                            when a.Action = '5' then '年度換卡'
                            else '' end) as                                                               ActionName,
                       --a.Action,
                       a.CardNo,
                       a.Backdate,
                       (case
                            when a.Reason = '1' then '招領逾期'
                            when a.Reason = '2' then '無此人'
                            when a.Reason = '3' then '址欠詳'
                            when a.Reason = '4' then '遷移不明'
                            when a.Reason = '5' then '拒收'
                            when a.Reason = '6' then '離職'
                            when a.Reason = '7' then '死亡'
                            when a.Reason = '8' then '信箱退租'
                            when a.Reason = '9' then '原因不明'
                            else '' end) as                                                               Reason,
                       a.Closedate,
                       (case
                            when a.Enditem = '0' then '自取'
                            when a.Enditem = '1' then '普掛'
                            when a.Enditem = '2' then '限掛'
                            when a.Enditem = '3' then '快遞'
                            when a.Enditem = '4' then '夜間投遞'
                            when a.Enditem = '5' then '註銷'
                            when a.Enditem = '6' then '碎卡'
                            else '' end) as                                                               Enditem
                from tbl_Card_BackInfo a
                         join tbl_Card_BaseInfo b
                              on a.cardno = b.cardno and a.action = b.action and a.id = b.id and a.trandate = b.trandate
                where a.CardBackStatus <> '2'
                order by a.Enddate, a.Action, a.serial_no
            end
";
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢需求功能SQL修改
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// 20210120 (U)by Joe, 增加羅馬拼音
    /// </summary>
    private const string SearchExport0518_1 = @"
        select a.serial_no,
               a.CustName,
               a.CustName_Roma,
               a.id,
               isnull((Select CardTypeName From tbl_CardType where CardType = a.cardtype), '信用卡') cardtype,
               (case
                    when a.Action = '1' then '新卡'
                    when a.Action = '2' then '掛失補發卡'
                    when a.Action = '3' then '毀損補發卡'
                    when a.Action = '4' then '補發密碼'
                    when a.Action = '5' then '年度換卡'
                    else '' end) as                                                               ActionName,
               --a.Action,
               a.CardNo,
               a.Backdate,
               (case
                    when a.Reason = '1' then '招領逾期'
                    when a.Reason = '2' then '無此人'
                    when a.Reason = '3' then '址欠詳'
                    when a.Reason = '4' then '遷移不明'
                    when a.Reason = '5' then '拒收'
                    when a.Reason = '6' then '離職'
                    when a.Reason = '7' then '死亡'
                    when a.Reason = '8' then '信箱退租'
                    when a.Reason = '9' then '原因不明'
                    else '' end) as                                                               Reason,
               a.Closedate,
               (case
                    when a.Enditem = '0' then '自取'
                    when a.Enditem = '1' then '普掛'
                    when a.Enditem = '2' then '限掛'
                    when a.Enditem = '3' then '快遞'
                    when a.Enditem = '4' then '夜間投遞'
                    when a.Enditem = '5' then '註銷'
                    when a.Enditem = '6' then '碎卡'
                    else '' end) as                                                               Enditem
        from tbl_Card_BackInfo a
                 join tbl_Card_BaseInfo b
                      on a.cardno = b.cardno and a.action = b.action and a.id = b.id and a.trandate = b.trandate
        where --a.CardBackStatus <> '2'
          --and
            (@datefrom is null or convert(datetime, a.ImportDate, 120) >= convert(datetime, @datefrom, 120))
          and (@dateto is null or convert(datetime, a.ImportDate, 120) <= convert(datetime, @dateto, 120))
        order by a.Enddate, a.Action, a.serial_no
";
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢需求功能SQL修改
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// 20210120 (U)by Joe, 增加羅馬拼音
    /// </summary>
    private const string SearchExport0518_2 = @"
            select a.serial_no,
                   a.CustName,
                   a.CustName_Roma,
                   a.id,
                   isnull((Select CardTypeName From tbl_CardType where CardType = a.cardtype), '信用卡') cardtype,
                   (case
                        when a.Action = '1' then '新卡'
                        when a.Action = '2' then '掛失補發卡'
                        when a.Action = '3' then '毀損補發卡'
                        when a.Action = '4' then '補發密碼'
                        when a.Action = '5' then '年度換卡'
                        else '' end) as                                                               ActionName,
                   --a.Action,
                   a.CardNo,
                   a.Backdate,
                   (case
                        when a.Reason = '1' then '招領逾期'
                        when a.Reason = '2' then '無此人'
                        when a.Reason = '3' then '址欠詳'
                        when a.Reason = '4' then '遷移不明'
                        when a.Reason = '5' then '拒收'
                        when a.Reason = '6' then '離職'
                        when a.Reason = '7' then '死亡'
                        when a.Reason = '8' then '信箱退租'
                        when a.Reason = '9' then '原因不明'
                        else '' end) as                                                               Reason,
                   a.Closedate,
                   (case
                        when a.Enditem = '0' then '自取'
                        when a.Enditem = '1' then '普掛'
                        when a.Enditem = '2' then '限掛'
                        when a.Enditem = '3' then '快遞'
                        when a.Enditem = '4' then '夜間投遞'
                        when a.Enditem = '5' then '註銷'
                        when a.Enditem = '6' then '碎卡'
                        else '' end) as                                                               Enditem
            from tbl_Card_BackInfo a
                     join tbl_Card_BaseInfo b
                          on a.cardno = b.cardno and a.action = b.action and a.id = b.id and a.trandate = b.trandate
            where a.CardBackStatus = '2'
              and (@datefrom is null or convert(datetime, a.Closedate, 120) >= convert(datetime, @datefrom, 120))
              and (@dateto is null or convert(datetime, a.Closedate, 120) <= convert(datetime, @dateto, 120))
            order by a.Enddate, a.Action, a.serial_no
";
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢需求功能SQL修改
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// 20210120 (U)by Joe, 增加羅馬拼音
    /// </summary>
    private const string SearchExport0518_3 = @"
                select a.serial_no,
                       a.CustName,
                       a.CustName_Roma,
                       a.id,
                       isnull((Select CardTypeName From tbl_CardType where CardType = a.cardtype), '信用卡') cardtype,
                       (case
                            when a.Action = '1' then '新卡'
                            when a.Action = '2' then '掛失補發卡'
                            when a.Action = '3' then '毀損補發卡'
                            when a.Action = '4' then '補發密碼'
                            when a.Action = '5' then '年度換卡'
                            else '' end) as                                                               ActionName,
                       --a.Action,
                       a.CardNo,
                       a.Backdate,
                       (case
                            when a.Reason = '1' then '招領逾期'
                            when a.Reason = '2' then '無此人'
                            when a.Reason = '3' then '址欠詳'
                            when a.Reason = '4' then '遷移不明'
                            when a.Reason = '5' then '拒收'
                            when a.Reason = '6' then '離職'
                            when a.Reason = '7' then '死亡'
                            when a.Reason = '8' then '信箱退租'
                            when a.Reason = '9' then '原因不明'
                            else '' end) as                                                               Reason,
                       a.Closedate,
                       (case
                            when a.Enditem = '0' then '自取'
                            when a.Enditem = '1' then '普掛'
                            when a.Enditem = '2' then '限掛'
                            when a.Enditem = '3' then '快遞'
                            when a.Enditem = '4' then '夜間投遞'
                            when a.Enditem = '5' then '註銷'
                            when a.Enditem = '6' then '碎卡'
                            else '' end) as                                                               Enditem
                from tbl_Card_BackInfo a
                         join tbl_Card_BaseInfo b
                              on a.cardno = b.cardno and a.action = b.action and a.id = b.id and a.trandate = b.trandate
                where a.CardBackStatus <> '2'
                order by a.Enddate, a.Action, a.serial_no
";

    #endregion

    #region SearchExport0520

    private const string SearchExport0520 = @"
select ROW_NUMBER() OVER(order by id, CardNo, UpdDate) as NUMBER,* from 
(select change.id,back.mailno,change.CardNo,change.UpdDate,change.UpdUser,change.OldAdd1,change.OldAdd2,change.OldAdd3,change.NewAdd1,change.NewAdd2,change.NewAdd3
from dbo.tbl_Card_DataChange change,dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base
where change.id=back.id and change.CardNo=back.CardNo and change.action=back.action and change.Trandate=back.Trandate
and change.id=base.id and change.CardNo=base.CardNo and change.action=base.action and change.Trandate=base.Trandate
and change.SourceType='2' and back.Enditem in ('1','2','3','4')
and change.UpdDate between @strUpdFrom and @strUpdTo 
and (base.mailno=@strMailNo or @strMailNo='NULL')
and (change.id=@strId or @strId='NULL')
union
select back.id,back.mailno,back.CardNo,back.Enddate as UpdDate,back.Enduid as UpdUser,back.OldAdd1,back.OldAdd2,back.OldAdd3,back.NewAdd1,back.NewAdd2,back.NewAdd3
from dbo.tbl_Card_BackInfo back ,dbo.tbl_Card_BaseInfo base
where base.id=back.id and base.CardNo=back.CardNo and base.action=back.action and base.Trandate=back.Trandate
and back.OriginalDBflg='1'and back.Enditem in ('1','2','3','4')
and back.Enddate between @strUpdFrom and @strUpdTo 
and (base.mailno=@strMailNo or @strMailNo='NULL')
and (back.id=@strId or @strId='NULL')
) U
order by id,CardNo,UpdDate
";

    #endregion

    #region SearchExport0207

    private const string SearchExport0207 = @"
Select ROW_NUMBER() OVER (ORDER BY IntoStore_Date desc) AS ROWID,
       IntoStore_Date,
       custname,
       OutStore_Status,
       IntoStoreCount,
       OutStoreFCount,
       OutStoreMCount,
       OutStoreDCount,
       DailyCloseCount,
       S.DailyClose_Date
From dbo.tbl_Card_DailyStockInfo S,
     dbo.tbl_Card_DailyClose C
Where S.DailyClose_Date = C.DailyCloseDate
  And C.DailyCloseDate = @DailyCloseDate
Order by IntoStore_Date desc
";

    #endregion

    #region SearchExport0401

    private const string SearchExport0401 = @"
SELECT tbl_Card_BaseInfo.ID,
       tbl_Card_BaseInfo.ACTION,
       tbl_Card_BaseInfo.cardno,
       tbl_Card_BaseInfo.add1,
       tbl_Card_BaseInfo.add2,
       tbl_Card_BaseInfo.add3,
--substring(tbl_Card_BaseInfo.mailno,1,6) mailno, 
       tbl_Card_BaseInfo.mailno,
       tbl_Card_BaseInfo.maildate,
       tbl_Card_BaseInfo.custname,
       tbl_Post.Uid,
       substring(CONVERT(varchar(10), convert(datetime, tbl_Card_BaseInfo.maildate), 112), 1, 4) as year,
       substring(CONVERT(varchar(10), convert(datetime, tbl_Card_BaseInfo.maildate), 112), 5, 2) as month,
       substring(CONVERT(varchar(10), convert(datetime, tbl_Card_BaseInfo.maildate), 112), 7, 2) as day
FROM tbl_Card_BaseInfo
         INNER JOIN
     tbl_Post ON tbl_Card_BaseInfo.cardno = tbl_Post.Cardno
WHERE (tbl_Card_BaseInfo.cardno = @cardno)
  AND (tbl_Card_BaseInfo.id = @id)
  AND (tbl_Card_BaseInfo.action = @action)
";

    #endregion

    #region SearchExport020501
    //20210120 (U)by Joe, 增加羅馬拼音
    private const string SearchExport020501 = @"
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, id) as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype),
              '信用卡')                                        as action,
       indate1,
       custname,
       custname_roma,
       name1,
       name1_roma,
       cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BaseInfo base
where kind = '1'
  and isnull(Urgency_Flg, '') <> '1'
  and (indate1 = @strMerchDateSQL or (indate1 < @strMerchDateSQL and isnull(IntoStore_Status, '0') = '0'))
  and (Merch_Code = @strMerch or @strMerch = 'NULL')
  and cardtype <> '900'
union
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, id) as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype),
              '信用卡')                                        as action,
       indate1,
       custname,
       custname_roma,
       name1,
       name1_roma,
       cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BaseInfo base
where kind = '1'
  and Urgency_Flg = '1'
  and (indate1 = @strMerchDate or (indate1 < @strMerchDate and isnull(IntoStore_Status, '0') = '0'))
  and (Merch_Code = @strMerch or @strMerch = 'NULL')
  and cardtype <> '900'
union
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, base.id) as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype),
              '信用卡')                                             as action,
       base.indate1,
       base.custname,
       base.custname_roma,
       base.name1,
       base.name1_roma,
       base.cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BackInfo back,
     dbo.tbl_Card_BaseInfo base
where back.action = base.action
  and back.id = base.id
  and back.cardno = base.cardno
  and back.trandate = base.trandate
  and isnull(base.kind, '') <> '1'
  and back.Enditem = '0'
  and ((InformMerchDate < @strMerchDate and isnull(IntoStore_Status, '0') <> '1') or
       (InformMerchDate = @strMerchDate and (isnull(IntoStore_Status, '0') <> '1' or
                                             (isnull(IntoStore_Status, '0') = '1' and
                                              IntoStore_Date >= back.ImportDate))))
  and (Merch_Code = @strMerch or @strMerch = 'NULL')
  and base.cardtype <> '900'
union
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, base.id) as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype),
              '信用卡')                                             as action,
       base.indate1,
       base.custname,
       base.custname_roma,
       base.name1,
       base.name1_roma,
       base.cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BackInfo back,
     dbo.tbl_Card_BaseInfo base
where back.action = base.action
  and back.id = base.id
  and back.cardno = base.cardno
  and back.trandate = base.trandate
  and isnull(base.kind, '') <> '1'
  and back.Enditem = '0'
  and (InformMerchDate = @strMerchDate or InformMerchDate < @strMerchDate)
  and isnull(IntoStore_Status, '0') = '1'
  and IntoStore_Date < back.ImportDate
  and (Merch_Code = @strMerch or @strMerch = 'NULL')
  and base.cardtype <> '900'          
";

    #endregion

    #region SearchExport020502
    //20210120 (U)by Joe, 增加羅馬拼音
    private const string SearchExport020502 = @"
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, id)                                  as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype), '信用卡') as action,
       indate1,
       custname,
       custname_roma,
       name1,
       name1_roma,
       cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BaseInfo base
where kind = '1'
  and isnull(Urgency_Flg, '') <> '1'
  and indate1 <= @strFetchDateSQL1
  and isnull(IntoStore_Status, '0') = '1'
  --//排除「已出庫、且出庫日已日結」的資料
  and (isnull(OutStore_Status, '0') = '0' or isnull(OutStore_Date, '') > (select top 1 DailyCloseDate
                                                                          from dbo.tbl_Card_DailyClose
                                                                          order by DailyCloseDate desc))
  and cardtype <> '900'
union
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, id)                                  as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype), '信用卡') as action,
       indate1,
       custname,
       custname_roma,
       name1,
       name1_roma,
       cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BaseInfo base
where kind = '1'
  and Urgency_Flg = '1'
  and indate1 <= @strFetchDateSQL2
  and isnull(IntoStore_Status, '0') = '1'
  --//排除「已出庫、且出庫日已日結」的資料
  and (isnull(OutStore_Status, '0') = '0' or isnull(OutStore_Date, '') > (select top 1 DailyCloseDate
                                                                          from dbo.tbl_Card_DailyClose
                                                                          order by DailyCloseDate desc))
  and cardtype <> '900'
union
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, base.id)                             as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype), '信用卡') as action,
       base.indate1,
       base.custname,
       base.custname_roma,
       base.name1,
       base.name1_roma,
       base.cardno,
       '□自取/□代取',
       '',
       '',
       '',
       ''
from dbo.tbl_Card_BackInfo back,
     dbo.tbl_Card_BaseInfo base
where back.action = base.action
  and back.id = base.id
  and back.cardno = base.cardno
  and back.trandate = base.trandate
  and isnull(base.kind, '') <> '1'
  and back.Enditem = '0'
  and InformMerchDate <= @strFetchDateSQL2
  and isnull(IntoStore_Status, '0') = '1'
  and IntoStore_Date >= back.ImportDate
  and base.cardtype <> '900'
        
";

    #endregion

    #region SearchExport0513

    private const string SearchExport0513 = @"
SELECT OTYPE, XC, CS, XC + CS as 'XC+CS', CG + SB as 'CG+SB', CG, SB
FROM (
         SELECT DISTINCT (CASE
                              WHEN M.NBLKCODE = 'B' THEN '強停(B)'
                              WHEN M.NBLKCODE = 'R' THEN '管制(R)'
                              WHEN M.NBLKCODE = 'S' THEN '管制(S)'
                              WHEN M.OBLKCODE = 'R' AND NBLKCODE = 'X' THEN '解管(R)'
                              ELSE '' END)          OTYPE,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE OBLKCODE = M.OBLKCODE AND NBLKCODE = M.NBLKCODE AND FILECODE = '0' AND
                                FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') XC,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE OBLKCODE = M.OBLKCODE AND NBLKCODE = M.NBLKCODE AND FILECODE IN ('1', '2') AND
                                FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') CS,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE OBLKCODE = M.OBLKCODE AND NBLKCODE = M.NBLKCODE AND SUCCESS_FLAG = '1' AND
                                FILECODE IN ('0', '1', '2') AND
                                FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') CG,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE OBLKCODE = M.OBLKCODE AND NBLKCODE = M.NBLKCODE AND SUCCESS_FLAG = '2' AND
                                FILECODE IN ('0', '1', '2') AND
                                FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') SB
         FROM (SELECT DISTINCT OBLKCODE, NBLKCODE, FILE_DATE, FILECODE FROM TBL_CANCELOASA_UD) M
         WHERE (M.FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
           AND M.FILECODE IN ('0', '1', '2')
           AND (@OUSER = 'NULL' OR 1 = 1)
     ) T0
";

    #endregion

    #region SearchExport0513_0

    private const string SearchExport0513_0 = @"
SELECT DISTINCT
(CASE
     WHEN NBLKCODE = 'B' THEN '強停(B)'
     WHEN NBLKCODE = 'R' THEN '管制(R)'
     WHEN NBLKCODE = 'S' THEN '管制(S)'
     WHEN OBLKCODE = 'R' AND NBLKCODE = 'X' THEN '解管(R)'
     ELSE '' END) AS BLKCODE
FROM TBL_CANCELOASA_UD
WHERE FILECODE IN ('0', '1', '2')
  AND SUCCESS_FLAG = @FLAG
  AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
  AND (@OUSER = 'NULL' OR 1 = 1)
  AND (@NUM = 'NULL' OR 1 = 1)
ORDER BY BLKCODE
";

    #endregion

    #region SearchExport0513_1

    private const string SearchExport0513_1 = @"
SELECT ROW_NUMBER() over (ORDER BY M.BLKCODE, M.OTYPE) AS ROW_NUM,*,'' AS O1,'' AS O2
FROM (
         SELECT (CASE WHEN FILECODE = '0' THEN '主機' WHEN FILECODE IN ('1', '2') THEN '催收GUI' ELSE '' END) AS OTYPE,
                SENDDATE,
                CARDNO,
                (CASE
                     WHEN NBLKCODE = 'B' THEN '強停(B)'
                     WHEN NBLKCODE = 'R' THEN '管制(R)'
                     WHEN NBLKCODE = 'S' THEN '管制(S)'
                     WHEN OBLKCODE = 'R' AND NBLKCODE = 'X' THEN '解管(R)'
                     ELSE '' END)                                                                         AS BLKCODE,
                MEMO,
                PERMITDATE,
                SYS_DATE
                -- UPDUSER
         FROM TBL_CANCELOASA_UD
         WHERE FILECODE IN ('0', '1', '2')
           AND SUCCESS_FLAG = @FLAG
           AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
           AND (@OUSER = 'NULL' OR 1 = 1)
           AND (@NUM = 'NULL' OR 1 = 1)
     ) M
WHERE M.BLKCODE = @BLKCODE
ORDER BY M.BLKCODE, M.OTYPE
";

    #endregion

    #region SearchExport0513_2

    private const string SearchExport0513_2 = @"
SELECT DISTINCT
(CASE
     WHEN NBLKCODE = 'B' THEN '強停(B)'
     WHEN NBLKCODE = 'R' THEN '管制(R)'
     WHEN NBLKCODE = 'S' THEN '管制(S)'
     WHEN OBLKCODE = 'R' AND NBLKCODE = 'X' THEN '解管(R)'
     ELSE '' END) AS BLKCODE
FROM TBL_CANCELOASA_UD
WHERE FILECODE IN ('0', '1', '2')
  AND SUCCESS_FLAG = @FLAG
  AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
  AND (@OUSER = 'NULL' OR 1 = 1)
  AND (@NUM = 'NULL' OR 1 = 1)
ORDER BY BLKCODE
";

    #endregion

    #region SearchExport0513_3

    private const string SearchExport0513_3 = @"
SELECT ROW_NUMBER() over (ORDER BY M.BLKCODE, M.OTYPE) AS ROW_NUM, *, '' AS O1, '' AS O2
FROM (
         SELECT (CASE WHEN FILECODE = '0' THEN '主機' WHEN FILECODE IN ('1', '2') THEN '催收GUI' ELSE '' END) AS OTYPE,
                SENDDATE,
                CARDNO,

                '主機擋掉'                                                                                    AS FAIL_REASON,
                (CASE
                     WHEN NBLKCODE = 'B' THEN '強停(B)'
                     WHEN NBLKCODE = 'R' THEN '管制(R)'
                     WHEN NBLKCODE = 'S' THEN '管制(S)'
                     WHEN OBLKCODE = 'R' AND NBLKCODE = 'X' THEN '解管(R)'
                     ELSE '' END)                                                                         AS BLKCODE,
                MEMO,
                -- OBLKCODE,
                REASON_CODE,
                ACTION_CODE,
                CWB_REGIONS
         FROM TBL_CANCELOASA_UD
         WHERE FILECODE IN ('0', '1', '2')
           AND SUCCESS_FLAG = @FLAG
           AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
           AND (@OUSER = 'NULL' OR 1 = 1)
           AND (@NUM = 'NULL' OR 1 = 1)
     ) M
WHERE M.BLKCODE = @BLKCODE
ORDER BY M.BLKCODE, M.OTYPE
";

    #endregion

    #region SearchExport0514

    private const string SearchExport0514 = @"
SELECT OTYPE, XC, CS, XC + CS as 'XC+CS', CG + SB as 'CG+SB', CG, SB
FROM (
         SELECT DISTINCT M.NBLKCODE                 OTYPE,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE NBLKCODE = M.NBLKCODE AND FILECODE = '3' AND FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') XC,
                         0 AS                       CS,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE NBLKCODE = M.NBLKCODE AND SUCCESS_FLAG = '1' AND FILECODE = '3' AND
                                FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') CG,
                         (SELECT COUNT(*)
                          FROM TBL_CANCELOASA_UD
                          WHERE NBLKCODE = M.NBLKCODE AND SUCCESS_FLAG = '2' AND FILECODE = '3' AND
                                FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE
                             OR @OSTARTDATE = 'NULL'
                             OR @OENDDATE = 'NULL') SB
         FROM (SELECT DISTINCT NBLKCODE, FILE_DATE, FILECODE FROM TBL_CANCELOASA_UD) M
         WHERE (M.FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
           AND M.FILECODE = '3'
           AND (@OUSER = 'NULL' OR 1 = 1)
     ) T0
";

    #endregion

    #region SearchExport0514_0

    private const string SearchExport0514_0 = @"
SELECT DISTINCT NBLKCODE
FROM TBL_CANCELOASA_UD
WHERE FILECODE = '3'
  AND SUCCESS_FLAG = @FLAG
  AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
  AND (@OUSER = 'NULL' OR 1 = 1)
  AND (@NUM = 'NULL' OR 1 = 1)
ORDER BY NBLKCODE
";

    #endregion

    #region SearchExport0514_1

    private const string SearchExport0514_1 = @"
SELECT ROW_NUMBER() OVER (ORDER BY NBLKCODE) AS ROW_NUM,
       '監控補掛' AS OTYPE,
       SENDDATE,
       CARDNO,
       NBLKCODE,
       MEMO,
       PERMITDATE,
       SYS_DATE,
       UPDUSER,
       '' AS O1
FROM TBL_CANCELOASA_UD
WHERE FILECODE = '3'
  AND SUCCESS_FLAG = @FLAG
  AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
  AND (@BLKCODE = 'NULL' OR NBLKCODE = @BLKCODE)
  AND (@OUSER = 'NULL' OR 1 = 1)
  AND (@NUM = 'NULL' OR 1 = 1)
ORDER BY NBLKCODE, OTYPE
";

    #endregion

    #region SearchExport0514_2

    private const string SearchExport0514_2 = @"
SELECT DISTINCT NBLKCODE
FROM TBL_CANCELOASA_UD
WHERE FILECODE = '3'
  AND SUCCESS_FLAG = @FLAG
  AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
  AND (@OUSER = 'NULL' OR 1 = 1)
  AND (@NUM = 'NULL' OR 1 = 1)
ORDER BY NBLKCODE
";

    #endregion

    #region SearchExport0514_3

    private const string SearchExport0514_3 = @"
SELECT ROW_NUMBER() over (ORDER BY NBLKCODE) AS ROW_NUM,
       '監控補掛'        AS OTYPE,
       SENDDATE,
       CARDNO,
       '主機檔掉，需補人工處理' AS FAIL_REASON,
       NBLKCODE,
       MEMO,
       REASON_CODE,
       ACTION_CODE,
       CWB_REGIONS,
       '' AS O1,
       '' AS O2
FROM TBL_CANCELOASA_UD
WHERE FILECODE = '3'
  AND SUCCESS_FLAG = @FLAG
  AND (FILE_DATE BETWEEN @OSTARTDATE AND @OENDDATE OR @OSTARTDATE = 'NULL' OR @OENDDATE = 'NULL')
  AND (@BLKCODE = 'NULL' OR NBLKCODE = @BLKCODE)
  AND (@OUSER = 'NULL' OR 1 = 1)
  AND (@NUM = 'NULL' OR 1 = 1)
ORDER BY NBLKCODE, OTYPE
";

    #endregion

    #region SearchExport0515

    private const string SearchExport0515 = @"
--1. 卡數 = CARDNO數量 + CARDNO2數量(不為空的話)
--2. 取卡方式為「包裹」，則同一製卡日、相同掛號號碼的卡片封數算為1;取卡方式不為「包裹」，則一張卡片封數算1(只計CARDNO,不計CARDNO2)
--CT3->CARDNO數量 CT1->CARDNO2數量  CT2 ->封數
SELECT kindName,
       (OT1 + ot3) AS OT1,
       OT2,
       (C1 + C3) AS C1,
       C2,
       (V1 + V3) AS V1,
       V2,
       (CE1 + CE3) AS CE1,
       CE2,
       (EC1 + EC3) AS EC1,
       EC2,
       (OT1 + ot3 + C1 + c3 + v1 + v3 + ce1 + ce3 + ec1 + ec3) AS A1,
       (ot2 + c2 + v2 + ce2 + ec2) AS A2
FROM (
         SELECT U1.KIND,
                U1.KINDORDER,
                U1.KINDNAME,
                U1.C1,
                CASE U1.C2 WHEN 0 THEN U1.C3 ELSE U1.C2 END    AS C2,
                U1.C3,
                U1.V1,
                CASE U1.V2 WHEN 0 THEN U1.V3 ELSE U1.V2 END    AS V2,
                U1.V3,
                U1.CE1,
                CASE U1.CE2 WHEN 0 THEN U1.CE3 ELSE U1.CE2 END AS CE2,
                U1.CE3,
                U1.EC1,
                CASE U1.EC2 WHEN 0 THEN U1.EC3 ELSE U1.EC2 END AS EC2,
                U1.EC3,
                U1.OT1,
                CASE U1.OT2 WHEN 0 THEN U1.OT3 ELSE U1.OT2 END AS OT2,
                U1.OT3
         FROM (
                  SELECT DISTINCT KIND,
                                  CONVERT(INT, KIND)                    KINDORDER,
                                  CASE M.KIND
                                      WHEN '0' THEN '0 普掛'
                                      WHEN '1' THEN '1 自取'
                                      WHEN '2' THEN '2 卡交介'
                                      WHEN '3' THEN '3 限掛'
                                      WHEN '4' THEN '4 快遞'
                                      WHEN '5' THEN '5 三天快速發卡'
                                      WHEN '6' THEN '6 保留'
                                      WHEN '7' THEN '7 其他'
                                      WHEN '8' THEN '8 包裹'
                                      WHEN '9' THEN '9 無法製卡'
                                      WHEN '10' THEN '10 卡片碎卡'
                                      WHEN '11' THEN '11 卡片註銷'
                                      WHEN '21' THEN '21 預製卡-無帳號'
                                      WHEN '22' THEN '22 預製卡-有帳號'
                                      WHEN '23' THEN '23 郵寄分行'
                                      WHEN '24' THEN '24 整批發薪'
                                      WHEN '25' THEN '25 RNMAIL' END AS KINDNAME,
                                  (SELECT COUNT(CARDNO2) CT1
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                                     AND ISNULL(CARDNO2, '') <> ''
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS C1,
                                  (
                                      SELECT COUNT(CF.INDATE1) CT2
                                      FROM (SELECT DISTINCT INDATE1, MAILNO, KIND
                                            FROM [TBL_CARD_BASEINFO] BASE
                                            WHERE KIND = '8'
                                              AND CARDTYPE IN (SELECT CARDTYPE
                                                               FROM TBL_CARDTYPE
                                                               WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                                              AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR
                                                   @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                                              AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR
                                                   @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                                              AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                           ) CF
                                      WHERE KIND = M.KIND
                                  )                                  AS C2,
                                  (SELECT COUNT(CARDNO) CT3
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS C3,
                                  (SELECT COUNT(CARDNO2) CT1
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                                     AND ISNULL(CARDNO2, '') <> ''
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS V1,
                                  (
                                      SELECT COUNT(VF.INDATE1) CT2
                                      FROM (SELECT DISTINCT INDATE1, MAILNO, KIND
                                            FROM [TBL_CARD_BASEINFO] BASE
                                            WHERE KIND = '8'
                                              AND CARDTYPE IN (SELECT CARDTYPE
                                                               FROM TBL_CARDTYPE
                                                               WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                                              AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR
                                                   @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                                              AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR
                                                   @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                                              AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                           ) VF
                                      WHERE KIND = M.KIND
                                  )                                  AS V2,
                                  (SELECT COUNT(CARDNO) CT3
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS V3,
                                  (SELECT COUNT(CARDNO2) CT1
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                                     AND ISNULL(CARDNO2, '') <> ''
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS CE1,
                                  (
                                      SELECT COUNT(CF.INDATE1) CT2
                                      FROM (SELECT DISTINCT INDATE1, MAILNO, KIND
                                            FROM [TBL_CARD_BASEINFO] BASE
                                            WHERE KIND = '8'
                                              AND CARDTYPE IN (SELECT CARDTYPE
                                                               FROM TBL_CARDTYPE
                                                               WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                                              AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR
                                                   @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                                              AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR
                                                   @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                                              AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                           ) CF
                                      WHERE KIND = M.KIND
                                  )                                  AS CE2,
                                  (SELECT COUNT(CARDNO) CT3
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS CE3,
                                  (SELECT COUNT(CARDNO2) CT1
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                                     AND ISNULL(CARDNO2, '') <> ''
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS EC1,
                                  (
                                      SELECT COUNT(CF.INDATE1) CT2
                                      FROM (SELECT DISTINCT INDATE1, MAILNO, KIND
                                            FROM [TBL_CARD_BASEINFO] BASE
                                            WHERE KIND = '8'
                                              AND CARDTYPE IN (SELECT CARDTYPE
                                                               FROM TBL_CARDTYPE
                                                               WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                                              AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR
                                                   @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                                              AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR
                                                   @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                                              AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                           ) CF
                                      WHERE KIND = M.KIND
                                  )                                  AS EC2,
                                  (SELECT COUNT(CARDNO) C3
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE IN (SELECT CARDTYPE
                                                      FROM TBL_CARDTYPE
                                                      WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS EC3,
                                  (SELECT COUNT(CARDNO2) CT1
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE NOT IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y')
                                     AND ISNULL(CARDNO2, '') <> ''
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS OT1,
                                  (
                                      SELECT COUNT(CF.INDATE1) CT2
                                      FROM (SELECT DISTINCT INDATE1, MAILNO, KIND
                                            FROM [TBL_CARD_BASEINFO] BASE
                                            WHERE KIND = '8'
                                              AND CARDTYPE NOT IN
                                                  (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y')
                                              AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR
                                                   @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                                              AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR
                                                   @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                                              AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                           ) CF
                                      WHERE KIND = M.KIND
                                  )                                  AS OT2,
                                  (SELECT COUNT(CARDNO) CT3
                                   FROM [TBL_CARD_BASEINFO] BASE
                                   WHERE KIND = M.KIND
                                     AND CARDTYPE NOT IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y')
                                     AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                          @MENDDATE = 'NULL')
                                     AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                          @PENDDATE = 'NULL')
                                     AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                                  )                                  AS OT3
                  FROM (SELECT DISTINCT KIND, MERCH_CODE, MAILDATE, INDATE1
                        FROM TBL_CARD_BASEINFO
                        WHERE KIND NOT IN ('21', '22', '23', '24')) M
                  WHERE (M.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                    AND (M.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                    AND (@FACTORY = '00' OR M.MERCH_CODE = @FACTORY)
              ) U1
         UNION
         SELECT DISTINCT KIND,
                         CONVERT(INT, KIND)                  KINDORDER,
                         CASE M.KIND
                             WHEN '22' THEN '22 預製卡-有帳號'
                             WHEN '23' THEN '23 郵寄分行'
                             WHEN '24' THEN '24 整批發薪' END AS KINDNAME,
                         (SELECT COUNT(CARDNO2) CT1
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                            AND ISNULL(CARDNO2, '') <> ''
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS C1,
                         0                                AS C2,
                         (SELECT COUNT(CARDNO) CT3
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS C3,
                         (SELECT COUNT(CARDNO2) CT1
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                            AND ISNULL(CARDNO2, '') <> ''
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS V1,
                         0                                AS V2,
                         (SELECT COUNT(CARDNO) CT3
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS V3,
                         (SELECT COUNT(CARDNO2) CT1
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                            AND ISNULL(CARDNO2, '') <> ''
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS CE1,
                         0                                AS CE2,
                         (SELECT COUNT(CARDNO) CT3
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS CE3,
                         (SELECT COUNT(CARDNO2) CT1
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                            AND ISNULL(CARDNO2, '') <> ''
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS EC1,
                         0                                AS EC2,
                         (SELECT COUNT(CARDNO) C3
                          FROM [TBL_CARD_BASEINFO] BASE
                          WHERE KIND = M.KIND
                            AND CARDTYPE IN
                                (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                            AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                                 @MENDDATE = 'NULL')
                            AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                                 @PENDDATE = 'NULL')
                            AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                         )                                AS EC3,
                         0                                AS OT1,
                         0                                AS OT2,
                         0                                AS OT3
         FROM (SELECT DISTINCT KIND, MERCH_CODE, MAILDATE, INDATE1
               FROM TBL_CARD_BASEINFO
               WHERE KIND IN ('22', '23', '24')) M
         WHERE (M.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
           AND (M.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
           AND (@FACTORY = '00' OR M.MERCH_CODE = @FACTORY)
         UNION
         SELECT KIND,
                CONVERT(INT, KIND) KINDORDER,
                '21 預製卡-無帳號' AS    KINDNAME,
                (SELECT COUNT(CARDNO2) CT1
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                   AND ISNULL(CARDNO2, '') <> ''
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    C1,
                0            AS    C2,
                (SELECT COUNT(CARDNO) CT3
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '1')
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    C3,
                (SELECT COUNT(CARDNO2) CT1
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                   AND ISNULL(CARDNO2, '') <> ''
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    V1,
                (SELECT COUNT(MAILNO)
                 FROM (SELECT DISTINCT MAILNO
                       FROM DBO.TBL_CARD_BASEINFO BASE
                       WHERE BASE.KIND IN ('21', '22', '23')
                         AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR
                              @MENDDATE = 'NULL')
                         AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR
                              @PENDDATE = 'NULL')
                         AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)) UM
                )            AS    V2,
                (SELECT COUNT(CARDNO) CT3
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '2')
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    V3,
                (SELECT COUNT(CARDNO2) CT1
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                   AND ISNULL(CARDNO2, '') <> ''
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    CE1,
                0            AS    CE2,
                (SELECT COUNT(CARDNO) CT3
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '3')
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    CE3,
                (SELECT COUNT(CARDNO2) CT1
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                   AND ISNULL(CARDNO2, '') <> ''
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    EC1,
                0            AS    EC2,
                (SELECT COUNT(CARDNO) C3
                 FROM [TBL_CARD_BASEINFO] BASE
                 WHERE KIND = M.KIND
                   AND CARDTYPE IN (SELECT CARDTYPE FROM TBL_CARDTYPE WHERE BANKCARDFLAG = 'Y' AND BANKCARDTYPE = '4')
                   AND (BASE.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
                   AND (BASE.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
                   AND (@FACTORY = '00' OR BASE.MERCH_CODE = @FACTORY)
                )            AS    EC3,
                0            AS    OT1,
                0            AS    OT2,
                0            AS    OT3
         FROM (SELECT DISTINCT KIND, MERCH_CODE, MAILDATE, INDATE1 FROM TBL_CARD_BASEINFO WHERE KIND = '21') M
         WHERE (M.INDATE1 BETWEEN @MSTATRDATE AND @MENDDATE OR @MSTATRDATE = 'NULL' OR @MENDDATE = 'NULL')
           AND (M.MAILDATE BETWEEN @PSTATRDATE AND @PENDDATE OR @PSTATRDATE = 'NULL' OR @PENDDATE = 'NULL')
           AND (@FACTORY = '00' OR M.MERCH_CODE = @FACTORY)
     ) U
ORDER BY U.KINDORDER        
";

    #endregion

    #region SearchExport0521

    private const string SearchExport0521 = @"
SELECT ROW_NUMBER() OVER (ORDER BY STOCK.MAILNO) AS ROWID,
       STOCK.MAILNO,
       SUBSTRING(LTRIM(RTRIM(BASE.CUSTNAME)), 2, LEN(LTRIM(RTRIM(BASE.CUSTNAME))) - 1) AS CUSTNAME,
       LTRIM(RTRIM(BASE.ZIP)) + LTRIM(RTRIM(BASE.ADD1))                                AS ZIP_ADD1,
       '' AS MEMO
FROM TBL_CARD_BASEINFO BASE,
     TBL_CARD_STOCKINFO STOCK
WHERE BASE.CARDNO = STOCK.CARDNO
  AND BASE.ACTION = STOCK.ACTION
  AND BASE.TRANDATE = STOCK.TRANDATE
  AND BASE.ID = STOCK.ID
  AND BASE.SELFPICK_TYPE = '4'
  AND BASE.SELFPICK_DATE = @SELFPICKDATE 
ORDER BY STOCK.MAILNO
";

    #endregion

    #region SearchExport0604

    private const string SearchExport0604 = @"
SELECT L.CREATE_DT,
       L.CREATE_USER,
       L.OPERATION_NAME,
       CASE L.TYPE_FLG
           WHEN 'A' THEN '新增'
           WHEN 'U' THEN '修改'
           WHEN 'D' THEN '刪除'
           WHEN 'IM' THEN '檔案匯入'
           WHEN 'OU' THEN '檔案匯出'
           END TYPE_FLG
FROM TBS_LOG L
WHERE L.CREATE_DT BETWEEN @DTSTART AND @DTEND
  AND (@FLG = 'NULL' OR L.TYPE_FLG = @FLG)
  AND (@USER = 'NULL' OR L.CREATE_USER LIKE @USER)
ORDER BY L.CREATE_DT DESC
";

    #endregion

    #region SearchExport0511

    private const string SearchExport0511 = @"
SELECT *
FROM (
--卡片異動
         SELECT A.INDATE1,
                CASE A.MERCH_CODE WHEN 'A' THEN '宏通' WHEN 'B' THEN '台銘' WHEN 'C' THEN '金雅拓' END AS MERCH_NAME,
                A.ID,
                A.CARDNO,
                (
                    CASE
                        WHEN B.OLDWAY = '0' THEN '普掛'
                        WHEN B.OLDWAY = '1' THEN '自取'
                        WHEN B.OLDWAY = '2' THEN '卡交介'
                        WHEN B.OLDWAY = '3' THEN '限掛'
                        WHEN B.OLDWAY = '4' THEN '快遞'
                        WHEN B.OLDWAY = '5' THEN '三天快速發卡'
                        WHEN B.OLDWAY = '6' THEN '保留'
                        WHEN B.OLDWAY = '7' THEN '其他'
                        WHEN B.OLDWAY = '8' THEN '包裹'
                        WHEN B.OLDWAY = '9' THEN '無法製卡'
                        WHEN B.OLDWAY = '10' THEN '卡片碎卡'
                        WHEN B.OLDWAY = '11' THEN '卡片註銷'
                        WHEN B.OLDWAY = '21' THEN '預製卡-無帳號'
                        WHEN B.OLDWAY = '22' THEN '預製卡-有帳號'
                        WHEN B.OLDWAY = '23' THEN '郵寄分行'
                        WHEN B.OLDWAY = '24' THEN '整批發薪'
                        WHEN B.OLDWAY = '25' THEN 'RNMAIL'
                        ELSE '' END
                    )                                                                           AS OLDWAY,
                (CASE
                     WHEN B.NEWWAY = '0' THEN '普掛'
                     WHEN B.NEWWAY = '1' THEN '自取'
                     WHEN B.NEWWAY = '2' THEN '卡交介'
                     WHEN B.NEWWAY = '3' THEN '限掛'
                     WHEN B.NEWWAY = '4' THEN '快遞'
                     WHEN B.NEWWAY = '5' THEN '三天快速發卡'
                     WHEN B.NEWWAY = '6' THEN '保留'
                     WHEN B.NEWWAY = '7' THEN '其他'
                     WHEN B.NEWWAY = '8' THEN '包裹'
                     WHEN B.NEWWAY = '9' THEN '無法製卡'
                     WHEN B.NEWWAY = '10' THEN '卡片碎卡'
                     WHEN B.NEWWAY = '11' THEN '卡片註銷'
                     WHEN B.NEWWAY = '21' THEN '預製卡-無帳號'
                     WHEN B.NEWWAY = '22' THEN '預製卡-有帳號'
                     WHEN B.NEWWAY = '23' THEN '郵寄分行'
                     WHEN B.NEWWAY = '24' THEN '整批發薪'
                     WHEN B.NEWWAY = '25' THEN 'RNMAIL'
                     ELSE '' END)                                                               AS NEWWAY,
                A.MAILNO,
                B.UPDDATE,
                B.UPDUSER
         FROM TBL_CARD_BASEINFO A
                  RIGHT JOIN TBL_CARD_DATACHANGE B
                             ON A.ID = B.ID AND A.CARDNO = B.CARDNO AND A.ACTION = B.ACTION AND A.TRANDATE = B.TRANDATE
         WHERE (@INDATEFROM IS NULL OR CONVERT(DATETIME, A.INDATE1, 120) >= CONVERT(DATETIME, @INDATEFROM, 120))
           AND (@INDATETO IS NULL OR CONVERT(DATETIME, A.INDATE1, 120) <= CONVERT(DATETIME, @INDATETO, 120))
           AND (B.URGENCYFLG = @URGENCYFLG OR @URGENCYFLG = 'NULL')
           AND (@NEWWAY = '00' OR B.NEWWAY = @NEWWAY)
           AND (@FACTORY = '00' OR A.MERCH_CODE = @FACTORY)
           AND OUTPUTFLG IN ('S', 'Y')
           AND A.KIND <> '9'
           AND ISNULL(SOURCETYPE, '') <> '2'
--UNION ALL
     ) U
ORDER BY UPDDATE DESC
";

    #endregion

    #region SearchExport0512_2

    private const string SearchExport0512_2 = @"
SELECT A.ID,
       A.CARDNO,
       A.TRANDATE,
       (CASE
            WHEN A.KIND = '0' THEN '普掛'
            WHEN A.KIND = '1' THEN '自取'
            WHEN A.KIND = '2' THEN '卡交介'
            WHEN A.KIND = '3' THEN '限掛'
            WHEN A.KIND = '4' THEN '快遞'
            WHEN A.KIND = '5' THEN '三天快速發卡'
            WHEN A.KIND = '6' THEN '保留'
            WHEN A.KIND = '7' THEN '其他'
            WHEN A.KIND = '8' THEN '包裹'
            WHEN A.KIND = '9' THEN '無法製卡'
            WHEN A.KIND = '10' THEN '卡片碎卡'
            WHEN A.KIND = '11' THEN '卡片註銷'
            WHEN A.KIND = '21' THEN '預製卡-無帳號'
            WHEN A.KIND = '22' THEN '預製卡-有帳號'
            WHEN A.KIND = '23' THEN '郵寄分行'
            WHEN A.KIND = '24' THEN '整批撥薪'
            WHEN A.KIND = '25' THEN 'RNMAIL'
            ELSE '' END)                                                                      AS KIND,
       (CASE WHEN A.MERCH_CODE = 'A' THEN '台銘' WHEN A.MERCH_CODE = 'B' THEN '宏通' ELSE '' END) AS MERCHCODE
FROM TBL_CARD_BASEINFO A
WHERE A.KIND NOT IN ('1', '2', '9', '10', '11')
  AND (ISNULL(A.MAILDATE, '') = '' OR ISNULL(A.MAILNO, '') = '')
  AND (A.INDATE1 BETWEEN @INDATESTART AND @INDATEEND OR @INDATESTART = 'NULL' OR @INDATEEND = 'NULL')
  AND A.CARD_FILE = @STRCARDFILE
";

    #endregion

    #region SearchExport0512_1

    private const string SearchExport0512_1 = @"
SELECT TB1.CARD_FILE,
       ISNULL(TB1.ALLNUM, 0)                       AS ALLNUM,
       ISNULL(TB2.SNUM, 0)                         AS SNUM,
       ISNULL(TB1.ALLNUM, 0) - ISNULL(TB2.SNUM, 0) AS FNUM
FROM (SELECT A.CARD_FILE, COUNT(*) AS ALLNUM
      FROM TBL_CARD_BASEINFO A
      WHERE A.KIND NOT IN ('1', '2', '9', '10', '11')
        AND (@INDATEFROM IS NULL OR CONVERT(DATETIME, A.INDATE1, 120) >= CONVERT(DATETIME, @INDATEFROM, 120))
        AND (@INDATETO IS NULL OR CONVERT(DATETIME, A.INDATE1, 120) <= CONVERT(DATETIME, @INDATETO, 120))
      GROUP BY A.CARD_FILE) TB1
         LEFT JOIN
     (SELECT A.CARD_FILE, COUNT(*) AS SNUM
      FROM TBL_CARD_BASEINFO A
      WHERE ISNULL(A.MAILDATE, '') <> ''
        AND ISNULL(A.MAILNO, '') <> ''
        AND A.KIND NOT IN ('1', '2', '9', '10', '11')
        AND (@INDATEFROM IS NULL OR CONVERT(DATETIME, A.INDATE1, 120) >= CONVERT(DATETIME, @INDATEFROM, 120))
        AND (@INDATETO IS NULL OR CONVERT(DATETIME, A.INDATE1, 120) <= CONVERT(DATETIME, @INDATETO, 120))
      GROUP BY A.CARD_FILE) TB2 ON TB1.CARD_FILE = TB2.CARD_FILE
              ";

    #endregion

    #region SearchExport0209

    private const string SearchExport0209 = @"
SELECT --CANCELOASAFILE,
       --CANCELOASADATE,
       --CASE @STRSOURCE WHEN '0' THEN 'OU檔註銷' WHEN '1' THEN '人工注銷' ELSE '退件注銷' END                          AS CANCELOASASOURCE,
       CARDNO,
       BLOCKLOG,
       MEMOLOG,
       CASE SFFLG WHEN '0' THEN '未注銷' WHEN '1' THEN '注銷成功' WHEN '2' THEN '注銷失敗' WHEN '3' THEN '人工注銷成功' END AS SFFLGNAME
       --CONVERT(VARCHAR(10), GETDATE(), 111)                                                                AS PRINTDATE
FROM DBO.TBL_CANCELOASA_DETAIL
WHERE CANCELOASAFILE = @STRFILE
  AND CANCELOASADATE = @STRDATE
";

    #endregion

    #endregion

    #region 無法製卡檔查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:無法製卡檔查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/28_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0502Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0502(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_05020000_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_05020000_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0502Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 1, "工作表1");  //NPOI起始ROW=1
            
            
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0502Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
    /// </summary>
    public static Boolean GetDataTable0502(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0502(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
    /// </summary>
    private static DataSet searchData0502(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0502
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion
    
    #region 換卡異動檔查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:換卡異動檔查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/28_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0503Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0503(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_05030000_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_05030000_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0503Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 1, "工作表1"); //NPOI起始ROW=1

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0503Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
    /// </summary>
    public static Boolean GetDataTable0503(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0503(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
    /// </summary>
    private static DataSet searchData0503(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0503
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion
    
    #region 郵局退件資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵局退件資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/28_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0504Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0504(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06050400_004";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgId = "06_06050400_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0504Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dtblSearchResult, ref wb, 1, "工作表1"); //NPOI 起始ROW=1

            ISheet sheet1 = wb.GetSheet("工作表1");
            //設定自動欄寬
            sheet1.AutoSizeColumn(1);
            sheet1.AutoSizeColumn(9);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0504Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();

            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
          
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    public static Boolean GetDataTable0504(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0504(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    private static DataSet searchData0504(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0504
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion
    
    #region 註銷作業報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:註銷作業報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0506Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0506(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06050600_000";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgId = "06_06050600_000";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0506Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI_filter(dtblSearchResult, ref wb, 1,1, "工作表1"); //NPOI起始ROW=1, Filter=1


            ////初始ROW位置
            int indexInSheetStart = 2;
            int indexInSheetEnd = indexInSheetStart;

            ////統計
            int successNum = 0;
            int failNum = 0;
            int totalNum = dtblSearchResult.Rows.Count;


            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 4];
            for (int intLoop = 0; intLoop < totalNum; intLoop++)
            {
                indexInSheetEnd++;

                // 卡號
                arrExportData[intLoop, 0] = dtblSearchResult.Rows[intLoop]["CardNo"].ToString();
                // BLOCK CODE
                arrExportData[intLoop, 1] = dtblSearchResult.Rows[intLoop]["BlockCode"].ToString();
                // MEMO
                arrExportData[intLoop, 2] = dtblSearchResult.Rows[intLoop]["Memo"].ToString();
                // 備註
                arrExportData[intLoop, 3] = dtblSearchResult.Rows[intLoop]["MemoLog"].ToString();
                // SFFlg
                if (dtblSearchResult.Rows[intLoop]["SFFlg"].ToString() == "1")
                {
                    successNum++;
                }
                else if (dtblSearchResult.Rows[intLoop]["SFFlg"].ToString() == "2")
                {
                    failNum++;
                }
            }
            string successNum_str = successNum.ToString();
            string failNum_str = failNum.ToString();
            string totalNum_str = totalNum.ToString();

            ISheet sheet1 = wb.GetSheet("工作表1");
            ISheet sheet2 = wb.GetSheet("頁尾");

            XSSFRow row = (XSSFRow)sheet1.CreateRow(sheet1.LastRowNum+1);
            var cell = row.CreateCell(0);
            var data = sheet2.GetRow(0).GetCell(0).StringCellValue.ToString();
            var data_style = sheet2.GetRow(0).GetCell(0).CellStyle;

            //取代字串
            for (int rc =0; rc< 3; rc++)
            {
                row = (XSSFRow)sheet1.CreateRow(sheet1.LastRowNum+rc);
                for(int cc = 0; cc < 3; cc++)
                {
                    cell = row.CreateCell(cc);
                    data = sheet2.GetRow(rc).GetCell(cc).StringCellValue.ToString();
                    if (data.Contains("$SuccessNum$"))
                    {
                        data = data.Replace("$SuccessNum$", successNum_str);
                    }else if (data.Contains("$FailNum$")){
                        data = data.Replace("$FailNum$", failNum_str);
                    }else if (data.Contains("$TotalNum$"))
                    {
                        data = data.Replace("$TotalNum$", totalNum_str);
                    }
                    data_style = sheet2.GetRow(rc).GetCell(cc).CellStyle;
                    cell.SetCellValue(data);
                    cell.CellStyle = data_style;
                    
                }
            }
            //刪除頁尾sheet
            wb.RemoveSheetAt(wb.GetSheetIndex("頁尾"));
            //設定自動欄寬
            sheet1.AutoSizeColumn(3);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0506Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();

            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
            
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    public static Boolean GetDataTable0506(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0506(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    private static DataSet searchData0506(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0506
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion
    
    #region 簡訊發送查詢報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:簡訊發送查詢報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0507Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0507(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06050700_000";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgId = "06_06050700_000";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0507Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dtblSearchResult, ref wb, 1, "工作表1"); //NPOI 起始ROW=1

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0507Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();

            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    public static Boolean GetDataTable0507(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0507(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    private static DataSet searchData0507(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0507
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 郵局寄送資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵局寄送資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0508Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            // 取要下載的資料

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0508(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06050800_000";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgId = "06_06050800_000";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0508Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dtblSearchResult, ref wb, 1, "工作表1"); //NPOI 起始ROW=1
            //設定自動欄寬
            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.AutoSizeColumn(4);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0508Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();

            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    public static Boolean GetDataTable0508(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0508(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    private static DataSet searchData0508(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0508
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    #endregion
    
    #region 製卡相關資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:製卡相關資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// 2020/12/10 Ares_Luke-業務需求調整NPOI to CSV
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0519Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0519(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051900_003";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051900_003";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            // string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
            //                           UtilHelper.GetAppSettings("ReportTemplate") + "0519Report.xlsx";
            // FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            // XSSFWorkbook wb = new XSSFWorkbook(fs);
            // ExportExcelForNPOI(dt, ref wb, 1, "製卡相關資料"); // NPOI 起始ROW=1
            //
            // //設定自動欄寬
            // //ISheet sheet1 = wb.GetSheet("製卡相關資料");
            // //for(int i=0; i < 23; i++)
            // //{
            // //    sheet1.AutoSizeColumn(i);
            // //}
            //
            // // 保存文件到程序運行目錄下
            // strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0519Report" + ".xlsx";
            // FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            // wb.Write(fs1);
            // fs1.Close();
            // fs.Close();

            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0519Report" + ".csv";
            String[] titleArr =
            {
                "身份證字號", "卡號", "製卡日", "退件日期", "Action", "Action名稱", "結案日", "結案原因代號", "結案原因名稱", "郵寄日期", "退件原因代碼",
                "退件原因名稱",
                "Card Type", "認同代碼", "Photo Type", "製卡廠名稱", "有效期限", "Take Card Flag", "取卡方式", "ZIP CODE", "地址", "分行",
                "序號"
            };

            return ToCsv(dt, titleArr , strPathFile);

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    public static Boolean GetDataTable0519(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0519(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    private static DataSet searchData0519(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0519
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            // 修改記錄:2020/11/17 Ares Luke 業務需求調整設定TimeOut為180(秒)
            sqlSearchData.CommandTimeout = 180;
            
            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 製卡相關資料查詢 - 記錄SQL
    /// <summary>
    /// 功能說明：執行報表產出時記錄SQL語法
    /// 作者：Ares_Stanley
    /// 創建時間：2020/12/29
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static bool recordSQL_0519(Dictionary<string, string> param)
    {
        StringBuilder sb = new StringBuilder(SearchExport0519.ToUpper());
        foreach (var data in param)
        {
            sb.Replace("@" + data.Key.ToUpper().ToString(), String.Format("'{0}'", data.Value));
        }
        Logging.Log(string.Format("\r\n========== 0519報表SQL Command_Start ==========\r\n{0}\r\n========== 0519報表SQL Command_End ==========", sb.ToString()));
        return true;
    }
    #endregion

    #region 退件日報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:退件日報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0516Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0516(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_05160000_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_05160000_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0516Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 4, "工作表1"); //NPOI 起始ROW=4

            //設定欄位資料
            ISheet sheet1 = wb.GetSheet("工作表1");
            string accessDate_str = string.Format("處理日期：{0}", param["Operaction"]);
            string printDate_str = string.Format("列印日期：{0}", DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(1).GetCell(0).SetCellValue(accessDate_str);
            sheet1.GetRow(1).GetCell(11).SetCellValue(printDate_str);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0516Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean GetDataTable0516(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0516(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    private static DataSet searchData0516(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0516
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 退件原因統計表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:退件原因統計表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/23_Ares_Stanley-增加公式預先計算; 2020/12/29_Ares_Stanley-獨立SQL紀錄
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0517Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0517(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_05170000_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_05170000_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0517Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 3, "工作表1"); // NPOI 起始ROW=3

            //添加日期至報表
            ISheet sheet1 = wb.GetSheet("工作表1");
            string accessDate_str = string.Format("統計日期：{0}~{1}", param["MstatrDate"], param["MendDate"]);
            string printDate_str = string.Format("列印日期：{0}", DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(1).GetCell(0).SetCellValue(accessDate_str);
            sheet1.GetRow(1).GetCell(6).SetCellValue(printDate_str);

            #region 文字格式
            XSSFCellStyle bf = (XSSFCellStyle)wb.CreateCellStyle();
            bf.BorderBottom = BorderStyle.Thin;
            bf.BorderLeft = BorderStyle.Thin;
            bf.BorderTop = BorderStyle.Thin;
            bf.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            bf.WrapText = true;
            //文字置中
            bf.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont bff = (XSSFFont)wb.CreateFont();
            //字體尺寸
            bff.FontHeightInPoints = 12;
            bff.FontName = "新細明體";
            bff.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            bf.SetFont(bff);
            #endregion 文字格式

            //新增總row
            sheet1.CreateRow(sheet1.LastRowNum + 1);
            sheet1.GetRow(sheet1.LastRowNum).CreateCell(0).SetCellValue("總計");
            sheet1.GetRow(sheet1.LastRowNum).GetCell(0).CellStyle = bf;
            string formu = ""; //公式字串

            // 轉換資料格式 "通用" to "數字"
            for(int row =3; row < sheet1.LastRowNum ; row++)
            {
                for(int col = 1; col < 7; col++)
                {
                    sheet1.GetRow(row).GetCell(col).SetCellValue(Int32.Parse(sheet1.GetRow(row).GetCell(col).StringCellValue.ToString()));
                }
            }
            // 設定公式，設定儲存格類型，設定儲存格樣式
            for(int col = 1; col < 7; col++)
            {
                switch (col)
                {
                    case 1: formu = string.Format("SUM(B4:B{0})", sheet1.LastRowNum); break;
                    case 2: formu = string.Format("SUM(C4:C{0})", sheet1.LastRowNum); break;
                    case 3: formu = string.Format("SUM(D4:D{0})", sheet1.LastRowNum); break;
                    case 4: formu = string.Format("SUM(E4:E{0})", sheet1.LastRowNum); break;
                    case 5: formu = string.Format("SUM(F4:F{0})", sheet1.LastRowNum); break;
                    case 6: formu = string.Format("SUM(G4:G{0})", sheet1.LastRowNum); break;
                }
                sheet1.GetRow(sheet1.LastRowNum).CreateCell(col).SetCellType(CellType.Formula);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(col).SetCellFormula(formu);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(col).CellStyle = bf;
            }
            XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0517Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/04
    /// </summary>
    public static Boolean GetDataTable0517(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0517(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/04
    /// </summary>
    private static DataSet searchData0517(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0517
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 退件原因統計表 - 記錄SQL
    /// <summary>
    /// 功能說明：執行報表產出時記錄SQL語法
    /// 作者：Ares_Stanley
    /// 創建時間：2020/12/29
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static bool recordSQL_0517(Dictionary<string, string> param)
    {
        StringBuilder sb = new StringBuilder(SearchExport0517.ToUpper());
        foreach(var data in param)
        {
            sb.Replace("@" + data.Key.ToUpper().ToString(), String.Format("'{0}'", data.Value));
        }
        Logging.Log(string.Format("\r\n========== 0517報表SQL Command_Start ==========\r\n{0}\r\n========== 0517報表SQL Command_End ==========", sb.ToString()));
        return true;
    }
    #endregion

    #region 退件連絡報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:退件連絡報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0518Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0518(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0518Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 3, "工作表1"); //NPOI起始ROW=3

            //添加日期至報表
            ISheet sheet1 = wb.GetSheet("工作表1");
            string accessDate_str = string.Format("統計日期：{0}~{1}", param["datefrom"], param["dateto"]);
            string printDate_str = string.Format("列印日期：{0}", DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(1).GetCell(0).SetCellValue(accessDate_str);
            sheet1.GetRow(1).GetCell(9).SetCellValue(printDate_str);
            //設定自動欄寬
            sheet1.AutoSizeColumn(5);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0518Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    public static Boolean GetDataTable0518(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0518(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    private static DataSet searchData0518(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            String val = param["backtype"];
            String sql = "";

            switch (val)
            {
                case "1": { sql = SearchExport0518_1; } break;
                case "2": { sql = SearchExport0518_2; } break;
                case "3": { sql = SearchExport0518_3; } break;
            }

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = sql
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    #endregion
    
    #region 址更重寄異動記錄查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:址更重寄異動記錄查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/10/29_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0520Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0520(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06052000_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06052000_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0520Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 3, "工作表1"); //NPOI 起始ROW=3

            //添加日期至報表
            ISheet sheet1 = wb.GetSheet("工作表1");
            string accessDate_str = string.Format("統計日期：{0}~{1}", param["strUpdFrom"], param["strUpdTo"]);
            string printDate_str = string.Format("列印日期：{0}", DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(1).GetCell(0).SetCellValue(accessDate_str);
            sheet1.GetRow(1).GetCell(11).SetCellValue(printDate_str);

            //設定自動欄寬
            for(int col=1; col<12; col ++)
            {
                sheet1.AutoSizeColumn(col);
            }

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0520Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    public static Boolean GetDataTable0520(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0520(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    private static DataSet searchData0520(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0520
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 分行郵寄資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:分行郵寄資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0509Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0509(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06050900_005";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06050900_005";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0509Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 1, "工作表1"); //NPOI 起始ROW=1

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0509Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    public static Boolean GetDataTable0509(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0509(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    private static DataSet searchData0509(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0509
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 扣卡明細查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:扣卡明細查詢 - Excel  
    /// 作    者:Ares Luke
    /// 創建時間:2020/06/23
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/06_Ares_Stanley-新增自動欄寬
    /// </summary>
    /// 
    public static bool CreateExcelFile_0510Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0510
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051000_006";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051000_006";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0510Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 1, "工作表1"); //NPOI起始ROW=1
            ISheet sheet1 = wb.GetSheet("工作表1");
            for(int col = 0; col < 7; col++)
            {
                sheet1.AutoSizeColumn(col);
            }
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0510Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region 自取庫存日結 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:自取庫存日結 - Excel  
    /// 作    者:Ares Luke
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// 創建時間:2020/06/29
    /// </summary>
    public static bool CreateExcelFile_0207Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0207(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06020701_001";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06020701_001";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0207Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI_filter(dt, ref wb, 4, 6,"test1"); //NPOI 起始ROW=4, filter=6


            //// 總筆數
            int totalRowsNum = dt.Rows.Count;
            // 報表欄位筆數
            //int totalColumnsNum = 4;

            int sumIntoStoreCount = 0;
            int sumOutStoreFCount = 0;
            int sumOutStoreMCount = 0;
            int sumOutStoreDCount = 0;
            int sumDailyCloseCount = 0;
            string dailyCloseDate = "";


            for (int intRowsLoop = 1; intRowsLoop <= totalRowsNum; intRowsLoop++)
            {
                if (intRowsLoop == 1)
                {
                    sumIntoStoreCount += (int)dt.Rows[intRowsLoop - 1][dt.Columns["IntoStoreCount"]];
                    sumOutStoreFCount += (int)dt.Rows[intRowsLoop - 1][dt.Columns["OutStoreFCount"]];
                    sumOutStoreMCount += (int)dt.Rows[intRowsLoop - 1][dt.Columns["OutStoreMCount"]];
                    sumOutStoreDCount += (int)dt.Rows[intRowsLoop - 1][dt.Columns["OutStoreDCount"]];
                    sumDailyCloseCount += (int)dt.Rows[intRowsLoop - 1][dt.Columns["DailyCloseCount"]];
                    dailyCloseDate = dt.Rows[intRowsLoop - 1][dt.Columns["DailyClose_Date"]].ToString();
                }
            }

            //當日庫存
            int preDailyCount = sumDailyCloseCount + sumOutStoreFCount + sumOutStoreMCount + sumOutStoreDCount -
                                sumIntoStoreCount;
            //設定欄位資料
            ISheet sheet1 = wb.GetSheet("test1");
            sheet1.GetRow(1).GetCell(3).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(2).GetCell(3).SetCellValue(dailyCloseDate);
            sheet1.GetRow(2).GetCell(4).SetCellValue(preDailyCount);
            sheet1.GetRow(2).GetCell(6).SetCellValue(sumIntoStoreCount);
            sheet1.GetRow(2).GetCell(8).SetCellValue(sumOutStoreFCount);
            sheet1.GetRow(2).GetCell(10).SetCellValue(sumOutStoreMCount);
            sheet1.GetRow(2).GetCell(12).SetCellValue(sumOutStoreDCount);
            sheet1.GetRow(2).GetCell(14).SetCellValue(sumDailyCloseCount);
            //設定自動欄寬
            for (int i = 0; i < 4; i++) { sheet1.AutoSizeColumn(i); }
            #region 文字格式cs
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式cs

            #region 文字格式bold
            XSSFCellStyle bold = (XSSFCellStyle)wb.CreateCellStyle();
            bold.BorderBottom = BorderStyle.Thin;
            bold.BorderLeft = BorderStyle.Thin;
            bold.BorderTop = BorderStyle.Thin;
            bold.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            bold.WrapText = true;
            //文字置中
            bold.VerticalAlignment = VerticalAlignment.Center;
            bold.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            XSSFFont boldf = (XSSFFont)wb.CreateFont();
            //字體尺寸
            boldf.FontHeightInPoints = 12;
            boldf.FontName = "新細明體";
            boldf.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            bold.SetFont(boldf);
            #endregion 文字格式bold
            //設定文字格式
            for (int row = 0; row < 3; row++)
            {
                for(int col = 0; col < 4; col++)
                {
                    sheet1.GetRow(row).GetCell(col).CellStyle = cs;
                }
            }
            for(int i = 0; i < 4; i++) { sheet1.GetRow(0).GetCell(i).CellStyle = bold; }
            sheet1.GetRow(1).GetCell(0).CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            sheet1.GetRow(2).GetCell(0).CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0207Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            strMsgId = "06_06020701_005";
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    public static Boolean GetDataTable0207(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0207(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    private static DataSet searchData0207(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0207
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion
    
    #region 郵局查單申請處理 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵局查單申請處理 - Excel  
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/06
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    public static bool CreateExcelFile_0401Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0401
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06040100_033";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06040100_033";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0401Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 0, "0401Report"); //NPOI 起始ROW=0
            #region 建立文字格式
            XSSFCellStyle a0 = (XSSFCellStyle)wb.CreateCellStyle();
            a0.BorderTop = BorderStyle.Medium;
            a0.BorderLeft = BorderStyle.Medium;
            a0.BorderRight = BorderStyle.None;

            XSSFCellStyle tb = (XSSFCellStyle)wb.CreateCellStyle();
            tb.BorderTop = BorderStyle.Medium;
            tb.BorderLeft = BorderStyle.None;
            tb.BorderRight = BorderStyle.None;

            XSSFCellStyle ds = (XSSFCellStyle)wb.CreateCellStyle();
            ds.BorderTop = BorderStyle.Medium;
            ds.BorderRight = BorderStyle.Medium;
            ds.BorderLeft = BorderStyle.None;
            #endregion 建立文字格式

            //新增、套用文字格式
            ISheet sheet1 = wb.GetSheet("0401Report");
            var row = sheet1.GetRow(0);
            sheet1.RemoveRow(row);
            sheet1.CreateRow(0);
            for (int i = 0; i < 123; i++)
            {
                sheet1.GetRow(0).CreateCell(i);
                sheet1.GetRow(0).GetCell(i).CellStyle = tb;
            }
            sheet1.GetRow(0).GetCell(21).CellStyle = a0;
            sheet1.GetRow(0).GetCell(122).CellStyle = ds;
            for (int irow = 0; irow < 5; irow++)
            {
                for (int icol = 0; icol < 3; icol++)
                {
                    sheet1.GetRow(irow).GetCell(icol).CellStyle = a0;
                }
            }
            sheet1.GetRow(0).HeightInPoints = 7;

            //日期
            sheet1.GetRow(24).GetCell(87).SetCellValue(Convert.ToString(int.Parse(DateTime.Now.ToString("yyyy")) - 1911));//交件年
            sheet1.GetRow(35).GetCell(88).SetCellValue(DateTime.Now.ToString("MM"));//交件月
            sheet1.GetRow(43).GetCell(88).SetCellValue(DateTime.Now.ToString("dd"));//交件日
            //收件人
            sheet1.GetRow(24).GetCell(53).SetCellValue(dt.Rows[0][dt.Columns["custname"]].ToString());
            //地址
            string addr = sheet1.GetRow(24).GetCell(50).StringCellValue.ToString();
            addr = addr.Replace("$add1$", dt.Rows[0][dt.Columns["add1"]].ToString());
            addr = addr.Replace("$add2$", dt.Rows[0][dt.Columns["add2"]].ToString());
            addr = addr.Replace("$add3$", dt.Rows[0][dt.Columns["add3"]].ToString());
            sheet1.GetRow(24).GetCell(50).SetCellValue(addr);//addr1,addr2,addr3

            //郵號件碼種類
            sheet1.GetRow(53).GetCell(115).SetCellValue(dt.Rows[0][dt.Columns["mailno"]].ToString());
            
            #endregion
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0401Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            strMsgId = "06_06040100_032";
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion
    
    #region 卡片自取逾期明細表_1 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:卡片自取逾期明細表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/08
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_020501Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            // 取要下載的資料

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport020501
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06020701_006";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06020701_006";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "020501Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 4, "工作表1"); // NPOI 起始ROW=4

            //統計
            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            ISheet sheet2 = wb.GetSheet("頁尾");
            // 新增row
            XSSFRow row = (XSSFRow)sheet1.CreateRow(sheet1.LastRowNum + 1);
            var cell = row.CreateCell(0);

            var late_data = sheet2.GetRow(0).GetCell(0).StringCellValue.ToString();
            var late_data_style = sheet2.GetRow(0).GetCell(0).CellStyle;
            //變更儲存格資料
            for (int rc = 0; rc < 3; rc++)
            {
                row = (XSSFRow)sheet1.CreateRow(sheet1.LastRowNum + rc);
                for (int cc = 0; cc < 11; cc++)
                {
                    cell = row.CreateCell(cc);
                    late_data = sheet2.GetRow(rc).GetCell(cc).StringCellValue.ToString();
                    if (late_data.Contains("$totalCardNo$"))
                    {
                        late_data = late_data.Replace("$totalCardNo$", totalNum.ToString());
                    }
                    late_data_style = sheet2.GetRow(rc).GetCell(cc).CellStyle;
                    cell.SetCellValue(late_data);
                    cell.CellStyle = late_data_style;

                }
            }
            //刪除頁尾sheet
            wb.RemoveSheetAt(wb.GetSheetIndex("頁尾"));
            //新增欄位資料
            sheet1.GetRow(1).GetCell(10).SetCellValue("自取時間：" + DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(2).GetCell(10).SetCellValue("列印日期：" + param["strMerchDate"]);
            //設置儲存格格式為文字
            for (int i =4; i < sheet1.LastRowNum-3; i++)
            {
                sheet1.GetRow(i).GetCell(5).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            }

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "020501Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region 卡片自取逾期明細表_2 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:卡片自取逾期明細表_2 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/08
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_020502Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            // 取要下載的資料

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport020502
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06020701_006";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];


            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06020701_006";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "020502Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 4, "工作表1");

            //統計
            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            ISheet sheet2 = wb.GetSheet("頁尾");

            XSSFRow row = (XSSFRow)sheet1.CreateRow(sheet1.LastRowNum + 1);
            var cell = row.CreateCell(0);
            var late_data = sheet2.GetRow(0).GetCell(0).StringCellValue.ToString();
            var late_data_style = sheet2.GetRow(0).GetCell(0).CellStyle;
            for (int rc = 0; rc < 3; rc++)
            {
                row = (XSSFRow)sheet1.CreateRow(sheet1.LastRowNum + rc);
                for (int cc = 0; cc < 11; cc++)
                {
                    cell = row.CreateCell(cc);
                    late_data = sheet2.GetRow(rc).GetCell(cc).StringCellValue.ToString();
                    if (late_data.Contains("$totalCardNo$"))
                    {
                        late_data = late_data.Replace("$totalCardNo$", totalNum.ToString());
                    }
                    late_data_style = sheet2.GetRow(rc).GetCell(cc).CellStyle;
                    cell.SetCellValue(late_data);
                    cell.CellStyle = late_data_style;

                }
            }
            wb.RemoveSheetAt(wb.GetSheetIndex("頁尾"));
            sheet1.GetRow(1).GetCell(10).SetCellValue("逾期時間："+param["strFetchDate"]);
            sheet1.GetRow(2).GetCell(10).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));

            for (int i = 4; i < sheet1.LastRowNum - 3; i++)
            {
                sheet1.GetRow(i).GetCell(5).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            }

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "020502Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region OASA管制解管批次作業量統計表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA管制解管批次作業量統計表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/10
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/12_Ares_Stanley-增加列印經辦; 2020/11/23_Ares_Stanley-增加公式預先計算;
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0513Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0513(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];


            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0513Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 4, "工作表1");

            int totalNum = dt.Rows.Count;
            #region 建立文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 建立文字格式

            ISheet sheet1 = wb.GetSheet("工作表1");
            //新增儲存格、套用格式
            sheet1.CreateRow(sheet1.LastRowNum + 1);
            for (int i = 0; i < 7; i++)
            {
                sheet1.GetRow(sheet1.LastRowNum).CreateCell(i);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).CellStyle = cs;
            }
            //新增合計欄
            sheet1.GetRow(sheet1.LastRowNum).GetCell(0).SetCellValue("合計");
            string formu = "";
            
            for (int i = 1; i < 7; i++)
            {
                for (int row = 4; row < sheet1.LastRowNum ; row++)
                {
                    sheet1.GetRow(row).GetCell(i).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                    sheet1.GetRow(row).GetCell(i).SetCellValue(Int32.Parse(sheet1.GetRow(row).GetCell(i).StringCellValue.ToString()));

                }
            }
            //設定公式，設定儲存格類型，套用公式
            for (int i = 1; i < 7; i++)
            {
                switch (i)
                {
                    case 1: formu = string.Format("SUM(B5:B{0})", sheet1.LastRowNum); break;
                    case 2: formu = string.Format("SUM(C5:C{0})", sheet1.LastRowNum); break;
                    case 3: formu = string.Format("SUM(D5:D{0})", sheet1.LastRowNum); break;
                    case 4: formu = string.Format("SUM(E5:E{0})", sheet1.LastRowNum); break;
                    case 5: formu = string.Format("SUM(F5:F{0})", sheet1.LastRowNum); break;
                    case 6: formu = string.Format("SUM(G5:G{0})", sheet1.LastRowNum); break;
                }
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellType(CellType.Formula);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellFormula(formu);
            }
            //設定欄位資料
            sheet1.GetRow(1).GetCell(0).SetCellValue("統計日期："+ param["OstartDate"] + "~" + param["OendDate"]);
            sheet1.GetRow(1).GetCell(5).SetCellValue("列印經辦：" + param["Ouser"]);
            sheet1.GetRow(2).GetCell(0).SetCellValue("批次日：" + DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(2).GetCell(5).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));

            XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0513Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean GetDataTable0513(Dictionary<String, String> param, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            Int32 count = 0;
            DataSet ds = searchData0513(param, ref count);
            if (null != ds)
            {
                dt = ds.Tables[0];
                count = dt.Rows.Count;
                DataRow row = dt.NewRow();
                row["OTYPE"] = "合計";
                String[] col = { "XC", "CS", "XC+CS", "CG+SB", "CG", "SB" };
                for(int i = 0; i < col.Length; i++)
                {
                    Int32 total = 0;
                    for (int j = 0; j < count; j++)
                    {
                        Int32 num;
                        if (Int32.TryParse(dt.Rows[j][col[i]].ToString(), out num))
                            total += num;
                    }
                    row[col[i]] = total;
                }
                dt.Rows.Add(row);
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    private static DataSet searchData0513(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0513
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region OASA管制解管批次作業量統計表_0 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA管制解管批次作業量統計表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/10
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/12_Ares_Stanley-修正報表錯誤
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0513_0Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            DataTable dt = new DataTable();
            if (!getPageType05130(ref param, ref strMsgId, ref dt))
                return false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0513_0Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 8, "工作表1"); // NPOI 起始ROW=8

            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式

            ISheet sheet1 = wb.GetSheet("工作表1");
            for (int row = 8; row < sheet1.LastRowNum + 1; row++)
            {
                if (sheet1.GetRow(row) != null)
                {
                    sheet1.RemoveRow(sheet1.GetRow(row));

                }
            }
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String blkCode = dt.Rows[i][dt.Columns["BLKCode"]].ToString();

                //* 查詢數據
                DataSet dstSearchData = searchData05130(param, blkCode);

                if (null != dstSearchData)
                {
                    if (dstSearchData.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt2 = dstSearchData.Tables[0];
                        if (i == 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, blkCode);
                        }
                        if (i > 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, blkCode);
                        }
                        ISheet sheetN = wb.GetSheet(blkCode);
                        for (int row = 8; row < 8 + dt2.Rows.Count; row++)
                        {
                            sheetN.CreateRow(row);
                            int dnum = row - 8;
                            for (int col = 0; col < 10; col++)
                            {
                                sheetN.GetRow(row).CreateCell(col).SetCellValue(dt2.Rows[dnum][col].ToString());
                                sheetN.GetRow(row).GetCell(col).CellStyle = cs;
                                sheetN.GetRow(row).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                            }

                        }

                        #region 匯入Excel文檔

                        int totalNum = dt2.Rows.Count;
                        sheetN.GetRow(0).GetCell(0).SetCellValue("OASA管制解管批次-" + (param["flag"] == "1" ? "成功報表" : "失敗報表") + "-" + blkCode);
                        sheetN.GetRow(1).GetCell(0).SetCellValue("檔案產出日：" + param["OstartDate"] + "~" + param["OendDate"]);
                        sheetN.GetRow(2).GetCell(0).SetCellValue("類型：" + blkCode);
                        sheetN.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + param["Ouser"]);
                        sheetN.GetRow(4).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
                        sheetN.GetRow(5).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 卡數：" + totalNum : "失敗" + " 卡數：" + totalNum);
                        sheetN.GetRow(6).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 總卡數：" + param["num"] : "失敗" + " 總卡數：" + param["num"]);
                        #endregion
                    }
                }
                
            }

            wb.RemoveSheetAt(0);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0513_0Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得分頁類別資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean getPageType05130(ref Dictionary<string, string> param, ref string strMsgId, ref DataTable dt)
    {
        #region 依據Request查詢資料庫

        //* 聲明SQL Command變量
        SqlCommand sqlSearchData = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText = SearchExport0513_0
        };

        foreach (var data in param)
        {
            SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
            sqlSearchData.Parameters.Add(paramStartDate);
        }

        //* 查詢數據
        DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

        #endregion 依據Request查詢資料庫

        #region 查無資料

        if (null == dstSearchData)
        {
            strMsgId = "06_05130000_005";
            return false;
        }

        dt = dstSearchData.Tables[0];

        if (dt.Rows.Count == 0)
        {
            strMsgId = "06_05130000_005";
            return false;
        }

        #endregion
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    private static DataSet searchData05130(Dictionary<String, String> param, String blkCode)
    {
        try
        {
            //* 聲明SQL Command變量
            SqlCommand tempSqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0513_1
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                tempSqlSearchData.Parameters.Add(paramStartDate);
            }

            tempSqlSearchData.Parameters.Add(new SqlParameter("@" + "BLKCode", blkCode));

            //* 查詢數據
            return SearchOnDataSet(tempSqlSearchData);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean GetDataTable05130(Dictionary<String, String> param, ref List<DataTable> list, ref List<String> name, ref String strMsgId)
    {
        try
        {
            DataTable dt = new DataTable();
            if (!getPageType05130(ref param, ref strMsgId, ref dt))
                return false;

            list = new List<DataTable>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String blkCode = dt.Rows[i][dt.Columns["BLKCode"]].ToString();

                //* 查詢數據
                DataSet ds = searchData05130(param, blkCode);
                if (null != ds)
                {
                    list.Add(ds.Tables[0]);
                    name.Add(blkCode);
                }
                else
                {
                    strMsgId = "06_05130000_005";
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            strMsgId = "06_05130000_002";
            Logging.Log(ex);
            return false;
        }
        return true;
    }

    #endregion

    #region OASA管制解管批次作業量統計表_2 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA管制解管批次作業量統計表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/15
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/12_Ares_Stanley-修正報表錯誤
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0513_2Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            DataTable dt = new DataTable();
            if (!getPageType05132(ref param, ref strMsgId, ref dt))
                return false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0513_2Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 8, "工作表1"); //NPOI 起始 ROW=8


            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式

            ISheet sheet1 = wb.GetSheet("工作表1");
            for (int row = 8; row < sheet1.LastRowNum+1 ; row++)
            {
                if (sheet1.GetRow(row) != null)
                {
                    sheet1.RemoveRow(sheet1.GetRow(row));

                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String blkCode = dt.Rows[i][dt.Columns["BLKCode"]].ToString();

                //* 查詢數據
                DataSet dstSearchData = searchData05132(param, blkCode);

                if (null != dstSearchData)
                {
                    if (dstSearchData.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt2 = dstSearchData.Tables[0];
                        if (i == 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, blkCode);
                        }
                        if (i > 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, blkCode);
                        }
                        ISheet sheetN = wb.GetSheet(blkCode);
                        for (int row = 8; row < 8 + dt2.Rows.Count; row++)
                        {
                            sheetN.CreateRow(row);
                            int dnum = row - 8;
                            for (int col = 0; col < 12; col++)
                            {
                                sheetN.GetRow(row).CreateCell(col).SetCellValue(dt2.Rows[dnum][col].ToString());
                                sheetN.GetRow(row).GetCell(col).CellStyle = cs;
                                sheetN.GetRow(row).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                            }

                        }

                        #region 匯入Excel文檔

                        int totalNum = dt2.Rows.Count;
                        sheetN.GetRow(0).GetCell(0).SetCellValue("OASA管制解管批次-" + (param["flag"] == "1" ? "成功報表" : "失敗報表") + "-" + blkCode);
                        sheetN.GetRow(1).GetCell(0).SetCellValue("檔案產出日：" + param["OstartDate"] + "~" + param["OendDate"]);
                        sheetN.GetRow(2).GetCell(0).SetCellValue("類型：" + blkCode);
                        sheetN.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + param["Ouser"]);
                        sheetN.GetRow(4).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
                        sheetN.GetRow(5).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 卡數：" + totalNum : "失敗" + " 卡數：" + totalNum);
                        sheetN.GetRow(6).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 總卡數：" + param["num"] : "失敗" + " 總卡數：" + param["num"]);
                        #endregion
                    }
                }
                
            }
            wb.RemoveSheetAt(0);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0513_2Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得分頁類別資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean getPageType05132(ref Dictionary<string, string> param, ref string strMsgId, ref DataTable dt)
    {
        #region 依據Request查詢資料庫

        //* 聲明SQL Command變量
        SqlCommand sqlSearchData = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText = SearchExport0513_2
        };

        foreach (var data in param)
        {
            SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
            sqlSearchData.Parameters.Add(paramStartDate);
        }

        //* 查詢數據
        DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

        #endregion 依據Request查詢資料庫

        #region 查無資料

        if (null == dstSearchData)
        {
            strMsgId = "06_05130000_005";
            return false;
        }

        dt = dstSearchData.Tables[0];

        if (dt.Rows.Count == 0)
        {
            strMsgId = "06_05130000_005";
            return false;
        }

        #endregion
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    private static DataSet searchData05132(Dictionary<String, String> param, String blkCode)
    {
        try
        {
            //* 聲明SQL Command變量
            SqlCommand tempSqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0513_3
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                tempSqlSearchData.Parameters.Add(paramStartDate);
            }

            tempSqlSearchData.Parameters.Add(new SqlParameter("@" + "BLKCode", blkCode));

            //* 查詢數據
            return SearchOnDataSet(tempSqlSearchData);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean GetDataTable05132(Dictionary<String, String> param, ref List<DataTable> list, ref List<String> name, ref String strMsgId)
    {
        try
        {
            DataTable dt = new DataTable();
            if (!getPageType05132(ref param, ref strMsgId, ref dt))
                return false;

            list = new List<DataTable>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String blkCode = dt.Rows[i][dt.Columns["BLKCode"]].ToString();

                //* 查詢數據
                DataSet ds = searchData05132(param, blkCode);
                if (null != ds)
                {
                    list.Add(ds.Tables[0]);
                    name.Add(blkCode);
                }
                else
                {
                    strMsgId = "06_05130000_005";
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            strMsgId = "06_05130000_002";
            Logging.Log(ex);
            return false;
        }
        return true;
    }

    #endregion

    #region OASA監控補掛報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA監控補掛報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/16
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/12_Ares_Stanley-增加列印經辦; 2020/11/23_Ares_Stanley-增加公式預先計算;
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0514Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0514(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];


            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0514Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 4, "工作表1"); // NPOI 起始 ROW =4

            int totalNum = dt.Rows.Count;

            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式

            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.CreateRow(sheet1.LastRowNum + 1);
            for (int i = 0; i < 7; i++)
            {
                sheet1.GetRow(sheet1.LastRowNum).CreateCell(i).CellStyle = cs;
            }
            sheet1.GetRow(sheet1.LastRowNum).GetCell(0).SetCellValue("合計");
            string formu = "";

            for (int i = 1; i < 7; i++)
            {
                for (int row = 4; row < sheet1.LastRowNum; row++)
                {
                    sheet1.GetRow(row).GetCell(i).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                    sheet1.GetRow(row).GetCell(i).SetCellValue(Int32.Parse(sheet1.GetRow(row).GetCell(i).StringCellValue.ToString()));

                }
            }

            for (int i = 1; i < 7; i++)
            {
                switch (i)
                {
                    case 1: formu = string.Format("SUM(B5:B{0})", sheet1.LastRowNum); break;
                    case 2: formu = string.Format("SUM(C5:C{0})", sheet1.LastRowNum); break;
                    case 3: formu = string.Format("SUM(D5:D{0})", sheet1.LastRowNum); break;
                    case 4: formu = string.Format("SUM(E5:E{0})", sheet1.LastRowNum); break;
                    case 5: formu = string.Format("SUM(F5:F{0})", sheet1.LastRowNum); break;
                    case 6: formu = string.Format("SUM(G5:G{0})", sheet1.LastRowNum); break;
                }
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellType(CellType.Formula);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellFormula(formu);
            }


            sheet1.GetRow(1).GetCell(0).SetCellValue("統計日期：" + param["OstartDate"] + "~" + param["OendDate"]);
            sheet1.GetRow(2).GetCell(0).SetCellValue("批次日：" + DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(2).GetCell(5).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(1).GetCell(5).SetCellValue("列印經辦：" + param["Ouser"]);
            XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0514Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean GetDataTable0514(Dictionary<String, String> param, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            Int32 count = 0;
            DataSet ds = searchData0514(param, ref count);
            if (null != ds)
            {
                dt = ds.Tables[0];
                count = dt.Rows.Count;
                DataRow row = dt.NewRow();
                row["OTYPE"] = "合計";
                String[] col = { "XC", "CS", "XC+CS", "CG+SB", "CG", "SB" };
                for (int i = 0; i < col.Length; i++)
                {
                    Int32 total = 0;
                    for (int j = 0; j < count; j++)
                    {
                        Int32 num;
                        if (Int32.TryParse(dt.Rows[j][col[i]].ToString(), out num))
                            total += num;
                    }
                    row[col[i]] = total;
                }
                dt.Rows.Add(row);
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    private static DataSet searchData0514(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0514
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region OASA監控補掛報表_0 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA監控補掛報表_0 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/16
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/12_Ares_Stanley-修正報表錯誤
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0514_0Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            DataTable dt = new DataTable();
            if (!getPageType05140(ref param, ref strMsgId, ref dt))
                return false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0514_0Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 8, "工作表1");  //NPOI起始ROW=8

            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式

            ISheet sheet1 = wb.GetSheet("工作表1");
            for (int row = 8; row < sheet1.LastRowNum; row++)
            {
                if (sheet1.GetRow(row) != null)
                {
                    sheet1.RemoveRow(sheet1.GetRow(row));

                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String nblkCode = dt.Rows[i][dt.Columns["NBLKCode"]].ToString();

                //* 查詢數據
                DataSet dstSearchData = searchData05140(param, nblkCode);
                
                if (null != dstSearchData)
                {
                    if (dstSearchData.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt2 = dstSearchData.Tables[0];
                        if (i == 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, nblkCode);
                        }
                        if (i > 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, nblkCode);
                        }
                        ISheet sheetN = wb.GetSheet(nblkCode);
                        for (int row = 8; row < 8 + dt2.Rows.Count; row++)
                        {
                            sheetN.CreateRow(row);
                            int dnum = row - 8;
                            for (int col = 0; col < 10; col++)
                            {
                                sheetN.GetRow(row).CreateCell(col).SetCellValue(dt2.Rows[dnum][col].ToString());
                                sheetN.GetRow(row).GetCell(col).CellStyle = cs;
                                sheetN.GetRow(row).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                            }
                        }
                        

                        #region 匯入Excel文檔

                        int totalNum = dt2.Rows.Count;
                        sheetN.GetRow(0).GetCell(0).SetCellValue("OASA監控補掛批次-" + (param["flag"] == "1" ? "成功報表" : "失敗報表") + "-" + nblkCode);
                        sheetN.GetRow(1).GetCell(0).SetCellValue("檔案產出日：" + param["OstartDate"] + "~" + param["OendDate"]);
                        sheetN.GetRow(2).GetCell(0).SetCellValue("類型：" + nblkCode);
                        sheetN.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + param["Ouser"]);
                        sheetN.GetRow(4).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
                        sheetN.GetRow(5).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 卡數：" + totalNum : "失敗" + " 卡數：" + totalNum);
                        sheetN.GetRow(6).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 總卡數：" + param["num"] : "失敗" + " 總卡數：" + param["num"]);
                        #endregion
                    }
                }
                
            }
            wb.RemoveSheetAt(0);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0514_0Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得分頁類別資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean getPageType05140(ref Dictionary<string, string> param, ref string strMsgId, ref DataTable dt)
    {
        #region 依據Request查詢資料庫

        //* 聲明SQL Command變量
        SqlCommand sqlSearchData = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText = SearchExport0514_0
        };

        foreach (var data in param)
        {
            SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
            sqlSearchData.Parameters.Add(paramStartDate);
        }

        //* 查詢數據
        DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

        #endregion 依據Request查詢資料庫

        #region 查無資料

        if (null == dstSearchData)
        {
            strMsgId = "06_05140000_005";
            return false;
        }

        dt = dstSearchData.Tables[0];

        if (dt.Rows.Count == 0)
        {
            strMsgId = "06_05140000_005";
            return false;
        }

        #endregion
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    private static DataSet searchData05140(Dictionary<String, String> param, String nblkCode = "")
    {
        try
        {
            //* 聲明SQL Command變量
            SqlCommand tempSqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0514_1
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                tempSqlSearchData.Parameters.Add(paramStartDate);
            }

            if(nblkCode != "")
                tempSqlSearchData.Parameters.Add(new SqlParameter("@" + "BLKCode", nblkCode));

            //* 查詢數據
            return SearchOnDataSet(tempSqlSearchData);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean GetDataTable05140(Dictionary<String, String> param, ref List<DataTable> list, ref List<String> name, ref String strMsgId)
    {
        try
        {
            DataTable dt = new DataTable();
            if (!getPageType05140(ref param, ref strMsgId, ref dt))
                return false;

            list = new List<DataTable>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String nblkCode = dt.Rows[i][dt.Columns["NBLKCode"]].ToString();

                //* 查詢數據
                DataSet ds = searchData05140(param, nblkCode);
                if (null != ds)
                {
                    list.Add(ds.Tables[0]);
                    name.Add(nblkCode);
                }
                else
                    return false;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
        return true;
    }

    #endregion

    #region OASA監控補掛報表_1 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA監控補掛報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/16
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0514_1Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            DataSet dstSearchData = searchData05140(param);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];


            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0514_0Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 8, "工作表1"); //NPOI起始ROW=8

            int totalNum = dt.Rows.Count;

            //#endregion
            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.GetRow(0).GetCell(0).SetCellValue("OASA監控補掛批次-" + (param["flag"] == "1" ? "成功報表" : "失敗報表") + "-" + param["BLKCode"]);
            sheet1.GetRow(1).GetCell(0).SetCellValue("檔案產出日：" + param["OstartDate"] + "~" + param["OendDate"]);
            sheet1.GetRow(2).GetCell(0).SetCellValue("類型：" + param["BLKCode"]);
            sheet1.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + param["Ouser"]);
            sheet1.GetRow(4).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(5).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 卡數：" + totalNum : "失敗" + " 卡數：" + totalNum);
            sheet1.GetRow(6).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 總卡數：" + param["num"] : "失敗" + " 總卡數：" + param["num"]);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0514_1Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean GetDataTable05141(Dictionary<String, String> param, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData05140(param);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }

    #endregion

    #region OASA監控補掛報表_2 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA監控補掛報表_2 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/16
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/12_Ares_Stanley-修正報表錯誤
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0514_2Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            DataTable dt = new DataTable();
            if (!getPageType05142(ref param, ref strMsgId, ref dt))
                return false;


            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0514_2Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 8, "工作表1"); //NPOI起始ROW=8

            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式

            ISheet sheet1 = wb.GetSheet("工作表1");
            for(int row = 8; row < sheet1.LastRowNum; row++)
            {
                if (sheet1.GetRow(row) != null)
                {
                    sheet1.RemoveRow(sheet1.GetRow(row));

                }
            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String nblkCode = dt.Rows[i][dt.Columns["NBLKCode"]].ToString();

                //* 查詢數據
                DataSet dstSearchData = searchData05142(param, nblkCode);
                
                if (null != dstSearchData)
                {
                    if (dstSearchData.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt2 = dstSearchData.Tables[0];
                        if (i == 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, nblkCode);
                        }
                        if (i > 0)
                        {
                            wb.CloneSheet(0);
                            wb.SetSheetName(i + 1, nblkCode);
                        }
                        ISheet sheetN = wb.GetSheet(nblkCode);
                        for (int row = 8; row < 8 + dt2.Rows.Count; row++)
                        {
                            sheetN.CreateRow(row);
                            int dnum = row - 8;
                            for (int col = 0; col < 12; col++)
                            {
                                sheetN.GetRow(row).CreateCell(col).SetCellValue(dt2.Rows[dnum][col].ToString());
                                sheetN.GetRow(row).GetCell(col).CellStyle = cs;
                                sheetN.GetRow(row).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                                sheetN.AutoSizeColumn(col);
                            }
                        }

                        #region 匯入Excel文檔

                        int totalNum = dt2.Rows.Count;
                        sheetN.GetRow(0).GetCell(0).SetCellValue("OASA監控補掛批次-" + (param["flag"] == "1" ? "成功報表" : "失敗報表") + "-" + nblkCode);
                        sheetN.GetRow(1).GetCell(0).SetCellValue("檔案產出日：" + param["OstartDate"] + "~" + param["OendDate"]);
                        sheetN.GetRow(2).GetCell(0).SetCellValue("類型：" + nblkCode);
                        sheetN.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + param["Ouser"]);
                        sheetN.GetRow(4).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
                        sheetN.GetRow(5).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 卡數：" + totalNum : "失敗" + " 卡數：" + totalNum);
                        sheetN.GetRow(6).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 總卡數：" + param["num"] : "失敗" + " 總卡數：" + param["num"]);
                        #endregion
                    }
                }
                
            }
            wb.RemoveSheetAt(0);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0514_2Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得分頁類別資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean getPageType05142(ref Dictionary<string, string> param, ref string strMsgId, ref DataTable dt)
    {
        #region 依據Request查詢資料庫

        //* 聲明SQL Command變量
        SqlCommand sqlSearchData = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText = SearchExport0514_2
        };

        foreach (var data in param)
        {
            SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
            sqlSearchData.Parameters.Add(paramStartDate);
        }

        //* 查詢數據
        DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

        #endregion 依據Request查詢資料庫

        #region 查無資料

        if (null == dstSearchData)
        {
            strMsgId = "06_05140000_005";
            return false;
        }

        dt = dstSearchData.Tables[0];

        if (dt.Rows.Count == 0)
        {
            strMsgId = "06_05140000_005";
            return false;
        }

        #endregion
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    private static DataSet searchData05142(Dictionary<String, String> param, String nblkCode = "")
    {
        try
        {
            //* 聲明SQL Command變量
            SqlCommand tempSqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0514_3
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                tempSqlSearchData.Parameters.Add(paramStartDate);
            }

            if (nblkCode != "")
                tempSqlSearchData.Parameters.Add(new SqlParameter("@" + "BLKCode", nblkCode));

            //* 查詢數據
            return SearchOnDataSet(tempSqlSearchData);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean GetDataTable05142(Dictionary<String, String> param, ref List<DataTable> list, ref List<String> name, ref String strMsgId)
    {
        try
        {
            DataTable dt = new DataTable();
            if (!getPageType05142(ref param, ref strMsgId, ref dt))
                return false;

            list = new List<DataTable>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String nblkCode = dt.Rows[i][dt.Columns["NBLKCode"]].ToString();

                //* 查詢數據
                DataSet ds = searchData05142(param, nblkCode);
                if (null != ds)
                {
                    list.Add(ds.Tables[0]);
                    name.Add(nblkCode);
                }
                else
                    return false;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
        return true;
    }

    #endregion

    #region OASA監控補掛報表_3 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:OASA監控補掛報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/16
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0514_3Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            DataSet dstSearchData = searchData05142(param);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];


            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051800_003";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0514_2Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 8, "工作表1"); //NPOI起始ROW=8

            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.GetRow(0).GetCell(0).SetCellValue("OASA監控補掛批次-" + (param["flag"] == "1" ? "成功報表" : "失敗報表") + "-" + param["BLKCode"]);
            sheet1.GetRow(1).GetCell(0).SetCellValue("檔案產出日：" + param["OstartDate"] + "~" + param["OendDate"]);
            sheet1.GetRow(2).GetCell(0).SetCellValue("類型：" + param["BLKCode"]);
            sheet1.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + param["Ouser"]);
            sheet1.GetRow(4).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
            sheet1.GetRow(5).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 卡數：" + totalNum : "失敗" + " 卡數：" + totalNum);
            sheet1.GetRow(6).GetCell(0).SetCellValue(param["flag"] == "1" ? "成功" + " 總卡數：" + param["num"] : "失敗" + " 總卡數：" + param["num"]);
            sheet1.AutoSizeColumn(4);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0514_3Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    public static Boolean GetDataTable05143(Dictionary<String, String> param, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData05142(param);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }

    #endregion

    #region 卡片數量統計表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:卡片數量統計表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/16
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/23_Ares_Stanley-增加公式預先計算;
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0515Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0515(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_05150000_009";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];


            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_05150000_009";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0515Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 5, "工作表1"); //NPOI起始ROW=5

            int totalNum = dt.Rows.Count;

            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);
            #endregion 文字格式
            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.CreateRow(sheet1.LastRowNum + 1);
            sheet1.GetRow(sheet1.LastRowNum).CreateCell(0).SetCellValue("合計");
            for(int i=5; i < sheet1.LastRowNum+1; i++)
            {
                sheet1.GetRow(i).GetCell(0).CellStyle = cs;
            }
            
            sheet1.AutoSizeColumn(0);
            string formu = "";
            for (int i = 1; i < 13; i++)
            {
                for (int row = 5; row < sheet1.LastRowNum; row++)
                {
                    sheet1.GetRow(row).GetCell(i).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                    sheet1.GetRow(row).GetCell(i).SetCellValue(Int32.Parse(sheet1.GetRow(row).GetCell(i).StringCellValue.ToString()));
                    sheet1.GetRow(row).GetCell(i).CellStyle = cs;
                }
            }
            //設定、套用公式
            for (int i = 1; i < 13; i++)
            {
                switch (i)
                {
                    case 1: formu = string.Format("SUM(B5:B{0})", sheet1.LastRowNum); break;
                    case 2: formu = string.Format("SUM(C5:C{0})", sheet1.LastRowNum); break;
                    case 3: formu = string.Format("SUM(D5:D{0})", sheet1.LastRowNum); break;
                    case 4: formu = string.Format("SUM(E5:E{0})", sheet1.LastRowNum); break;
                    case 5: formu = string.Format("SUM(F5:F{0})", sheet1.LastRowNum); break;
                    case 6: formu = string.Format("SUM(G5:G{0})", sheet1.LastRowNum); break;
                    case 7: formu = string.Format("SUM(H5:H{0})", sheet1.LastRowNum); break;
                    case 8: formu = string.Format("SUM(I5:I{0})", sheet1.LastRowNum); break;
                    case 9: formu = string.Format("SUM(J5:J{0})", sheet1.LastRowNum); break;
                    case 10: formu = string.Format("SUM(K5:K{0})", sheet1.LastRowNum); break;
                    case 11: formu = string.Format("SUM(L5:L{0})", sheet1.LastRowNum); break;
                    case 12: formu = string.Format("SUM(M5:M{0})", sheet1.LastRowNum); break;
                }
                sheet1.GetRow(sheet1.LastRowNum).CreateCell(i);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).CellStyle = cs;
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellType(CellType.Formula);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellFormula(formu);
                sheet1.AutoSizeColumn(i);
            }

            sheet1.GetRow(0).GetCell(0).SetCellValue("卡片數量統計表 - " + param["FactoryName"]);
            sheet1.GetRow(1).GetCell(0).SetCellValue("統計日期：" + param["CountS"] + "~" + param["CountE"]);
            sheet1.GetRow(1).GetCell(12).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));

            XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0515Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    public static Boolean GetDataTable0515(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0515(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    private static DataSet searchData0515(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0515
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 自取改限掛大宗掛號單 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:自取改限掛大宗掛號單 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/23
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0521Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0521(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06052100_002";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06052100_002";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0521Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 6, "工作表1"); //NPOI起始ROW=6

            int totalNum = dt.Rows.Count;
            #region 文字格式
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            XSSFCellStyle ra = (XSSFCellStyle)wb.CreateCellStyle();
            ra.BorderBottom = BorderStyle.Thin;
            ra.BorderLeft = BorderStyle.Thin;
            ra.BorderTop = BorderStyle.Thin;
            ra.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            ra.WrapText = true;
            //文字置中
            ra.VerticalAlignment = VerticalAlignment.Center;
            ra.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            #endregion

            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.CreateRow(sheet1.LastRowNum + 1);
            for (int i = 0; i < 5; i++) 
            {
                sheet1.GetRow(sheet1.LastRowNum).CreateCell(i);
                sheet1.GetRow(sheet1.LastRowNum).GetCell(i).CellStyle = cs;
                switch (i)
                {
                    case 1:
                        sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellValue("上開掛號函件共　" + totalNum + "　件照收無誤");
                        sheet1.GetRow(sheet1.LastRowNum).GetCell(i).CellStyle = ra;
                        break;
                    case 4:
                        sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellValue("共 " + totalNum + " 卡");
                        break;
                }
            }
            sheet1.CreateRow(sheet1.LastRowNum + 1);
            for (int i = 0; i < 5; i++)
            {
                sheet1.GetRow(sheet1.LastRowNum).CreateCell(i).CellStyle = cs;
                if (i == 1) 
                { 
                    sheet1.GetRow(sheet1.LastRowNum).GetCell(i).SetCellValue("郵資共計           元");
                    sheet1.GetRow(sheet1.LastRowNum).GetCell(i).CellStyle = ra;
                }
            }
            sheet1.GetRow(2).GetCell(0).SetCellValue("日期:" + param["SelfPickDate"] + "　空號 : ");
            sheet1.SetColumnWidth(1, (int)((37 + 0.72) * 256));


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0521Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/10
    /// </summary>
    public static Boolean GetDataTable0521(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0521(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/10
    /// </summary>
    private static DataSet searchData0521(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0521
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 操作LOG紀錄 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:操作LOG紀錄 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/23
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0604Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0604
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06052100_002";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06052100_002";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0604Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 2, "工作表1");

            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.SetColumnWidth(0, (int)(21.5 + 0.72) * 256);
            sheet1.AutoSizeColumn(2);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0604Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region 更改寄送方式記錄查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:更改寄送方式記錄查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/24
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI; 2020/11/09_Ares_Stanley-修正欄寬
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0511Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            //* 查詢數據
            Int32 count = 0;
            DataSet dstSearchData = searchData0511(param, ref count);

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051200_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051200_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0511Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 1, "工作表1"); //NPOI起始ROW=1

            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");

            for (int col = 0; col < 9; col++) { sheet1.AutoSizeColumn(col); }


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0511Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    public static Boolean GetDataTable0511(Dictionary<String, String> param, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
    {
        try
        {
            //* 查詢數據
            DataSet ds = searchData0511(param, ref count, idx, size);
            if (null != ds)
            {
                dt = ds.Tables[0];
                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢共用取得資料需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    private static DataSet searchData0511(Dictionary<String, String> param, ref Int32 count, Int32 idx = -1, Int32 size = -1)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0511
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            if (idx >= 0)
                return SearchOnDataSet(sqlSearchData, idx, size, ref count);
            else
                return SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion

    #region 郵件交寄狀況檢核-2 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵件交寄狀況檢核-2 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/24
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0512_2Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0512_2
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051200_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051200_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0512_2Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 2, "工作表1"); //NPOI起始ROW=2

            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            //設定欄寬
            sheet1.SetColumnWidth(1, (int)((25.5 + 0.72) * 256));
            for(int row = 2; row < sheet1.LastRowNum + 1; row++)
            {
                for(int col = 0; col < 5; col++)
                {
                    sheet1.GetRow(row).GetCell(col).CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                }
            }
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0512_2Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region 郵件交寄狀況檢核-1- Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵件交寄狀況檢核-1 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/24
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0512_1Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0512_1
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06051200_004";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06051200_004";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0512_1Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 1, "工作表1"); //NPOI起始ROW=1

            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.SetColumnWidth(0,(int)((25+0.72)*256));

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0512_1Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region 註銷記錄處理>記錄確認 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:註銷記錄處理>記錄確認 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/24
    /// 修改紀錄:2020/11/04_Ares_Stanley-更改報表產出方式為NPOI
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0209Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        
        
        try
        {
            // 檢查目錄
            CSIPCommonModel.BaseItem.Function.CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0209
            };

            foreach (var data in param)
            {
                SqlParameter paramStartDate = new SqlParameter("@" + data.Key, data.Value);
                sqlSearchData.Parameters.Add(paramStartDate);
            }

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgId = "06_06020701_006";
                return false;
            }

            DataTable dt = dstSearchData.Tables[0];

            if (dt.Rows.Count == 0)
            {
                strMsgId = "06_06020701_006";
                return false;
            }

            #endregion

            #region 匯入Excel文檔

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "0209Report.xlsx";
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            XSSFWorkbook wb = new XSSFWorkbook(fs);
            ExportExcelForNPOI(dt, ref wb, 5, "工作表1"); //NPOI起始ROW=5

            int totalNum = dt.Rows.Count;

            ISheet sheet1 = wb.GetSheet("工作表1");
            sheet1.GetRow(1).GetCell(0).SetCellValue("註銷時間：" + param["strDate"]);
            // 來源
            if (param["strSource"] == "0")
            {
                sheet1.GetRow(2).GetCell(0).SetCellValue("來源：OU檔註銷");
            }
            else if (param["strSource"] == "1")
            {
                sheet1.GetRow(2).GetCell(0).SetCellValue("來源：人工註銷");
            }
            else
            {
                sheet1.GetRow(2).GetCell(0).SetCellValue("來源：退件註銷");
            }
            sheet1.GetRow(3).GetCell(0).SetCellValue("檔名：" + param["strFile"]);
            sheet1.GetRow(3).GetCell(3).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyy/MM/dd"));

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0209Report" + ".xlsx";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
        finally
        {
        }
    }

    #endregion

    #region 匯入EXCEL資料(新版)

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:共用匯入EXCEL資料 - Excel
    /// 作    者:Ares Luke
    /// 創建時間:2020/08/06
    /// 修改時間：2020/11/04_Ares_Stanley-新增NPOI、移除原有ExcelExport function
    /// </summary>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>

    private static void ExportExcelForNPOI(DataTable dt, ref XSSFWorkbook wb, Int32 start, String sheetName)
    {
        try
        {
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if(dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count;
                foreach(DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for(int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    private static void ExportExcelForNPOI_filter(DataTable dt, ref XSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
    {
        try
        {
            XSSFCellStyle cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;
           
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;

            XSSFFont font1 = (XSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count-delColumn;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for (int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:將DataTable to Csv 
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/10
    /// 修改紀錄:
    /// </summary>
    /// <param name="dt">資料</param>
    /// <param name="titleArr">資料表抬頭</param>
    /// <param name="strFilePath">儲存路徑</param>
    /// <returns></returns>
    private static bool ToCsv(DataTable dt, string[] titleArr , string strFilePath)
    {
        try
        {
            //檢查長度是否相符
            if (titleArr.Length != dt.Columns.Count)
            {
                return false;
            }

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.GetEncoding("UTF-8"));

            //headers
            for (int i = 0; i < titleArr.Length; i++)
            {
                sw.Write(titleArr[i]);
                if (i < dt.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }

            sw.Write(sw.NewLine);

            //detail
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        value = String.Format("=\"{0}\"", value); 
                        sw.Write(value);
                    }
                    if (i < dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
    }

}