using CSIPCommonModel.BusinessRules;
using EntityLayer;
using Framework.Common.Logging;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using DataTable = System.Data.DataTable;
using ExcelApplication = Microsoft.Office.Interop.Excel.ApplicationClass;

/// <summary>
/// BR_Excel_File 的摘要描述
/// </summary>
public class BR_Excel_File : BRBase<Entity_UnableCard>
{
    #region 各報表SQL語句

    #region SearchExport0502

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
      where (@maildatefrom is null or convert(datetime, maildate, 120) >= convert(datetime, @maildatefrom, 120))
        and (@maildateto is null or convert(datetime, maildate, 120) <= convert(datetime, @maildateto, 120))
        and branch_id = @branchid
        --and kind='23'
        and kind in ('21', '22', '23', '24')
      GROUP BY maildate, cardtype) t
GROUP BY mailno, maildate, cardtype
ORDER BY maildate DESC
";

    #endregion
    
    #region SearchExport0510

    private const string SearchExport0510 = @"
select id,custname,cardno,indate1,UpdDate,CNote, kktime 
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
       a.kind,
       a.expdate,
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
from tbl_Card_BaseInfo a
         left join tbl_Card_BackInfo b on
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
";

    #endregion

    #region SearchExport0516

    private const string SearchExport0516 = @"
select ReasonName, sum(DayIn) DayIn,sum(DayOut) DayOut,sum(CountEnd) CountEnd,sum(END0) END0
,sum(END1) END1,sum(END2) END2,sum(END3) END3,sum(END4) END4,sum(END5) END5,sum(END6) END6,sum(END7) END7
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
select distinct  m.Reason,
  CASE m.Reason WHEN '1' THEN '招領逾期' WHEN '2' THEN '無此人' 
WHEN '3' THEN '址欠詳' WHEN '4' THEN '遷移不明' 
WHEN '5' THEN '拒收' WHEN '6' THEN '離職'
WHEN '7' THEN '死亡' WHEN '8' THEN '信箱退租'
WHEN '9' THEN '原因不明' 
 END as ReasonName,
(select count(cardno) from tbl_Card_BackInfo 
	where Reason=m.Reason and Action='1' and (Action = @Action OR @Action = 'NULL')  and Backdate BETWEEN @MstatrDate AND @MendDate) as Action1,
(select count(cardno) from tbl_Card_BackInfo 
	where Reason=m.Reason and Action='2' and (Action = @Action OR @Action = 'NULL')  and Backdate BETWEEN @MstatrDate AND @MendDate ) as Action2,
(select count(cardno) from tbl_Card_BackInfo 
	where Reason=m.Reason and Action='3' and (Action = @Action OR @Action = 'NULL')  and Backdate BETWEEN @MstatrDate AND @MendDate ) as Action3,
(select count(cardno) from tbl_Card_BackInfo 
	where Reason=m.Reason and Action='4' and (Action = @Action OR @Action = 'NULL')   and Backdate BETWEEN @MstatrDate AND @MendDate) as Action4,
(select count(cardno) from tbl_Card_BackInfo 
	where Reason=m.Reason and Action='5'  and (Action = @Action OR @Action = 'NULL')  and Backdate BETWEEN @MstatrDate AND @MendDate) as Action5
from 
(
select Reason
from tbl_Card_BackInfo 
where
(Backdate BETWEEN @MstatrDate AND @MendDate ) AND
(Action = @Action OR @Action = 'NULL')
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

    private const string SearchExport020501 = @"
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, id) as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype),
              '信用卡')                                        as action,
       indate1,
       custname,
       name1,
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
       name1,
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
       base.name1,
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
       base.name1,
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

    private const string SearchExport020502 = @"
select ROW_NUMBER() over (ORDER BY IntoStore_Date DESC, id)                                  as no,
       isnull((Select CardTypeName From tbl_CardType where CardType = base.cardtype), '信用卡') as action,
       indate1,
       custname,
       name1,
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
       name1,
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
       base.name1,
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

    #endregion

    #region 共用 檢查目錄路徑

    #region 檢查路徑是否存在，存在刪除該路徑下所有的文檔資料

    /// <summary>
    /// 檢查路徑是否存在，存在刪除該路徑下所有的文檔資料
    /// </summary>
    /// <param name="strPath"></param>
    public static void CheckDirectory(ref string strPath)
    {
        try
        {
            string strOldPath = strPath;
            //* 判斷路徑是否存在
            strPath = strPath + "\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(strPath))
            {
                //* 如果不存在，創建路徑
                Directory.CreateDirectory(strPath);
            }

            //* 取該路徑下所有路徑
            string[] strDirectories = Directory.GetDirectories(strOldPath);
            for (int intLoop = 0; intLoop < strDirectories.Length; intLoop++)
            {
                if (strDirectories[intLoop] != strPath)
                {
                    if (Directory.Exists(strDirectories[intLoop]))
                    {
                        // * 刪除目錄下的所有文檔
                        DirectoryInfo di = new DirectoryInfo(strDirectories[intLoop]);
                        FileSystemInfo[] fsi = di.GetFileSystemInfos();
                        for (int intIndex = 0; intIndex < fsi.Length; intIndex++)
                        {
                            FileInfo fi = fsi[intIndex] as FileInfo;
                            if (fi != null)
                            {
                                fi.Delete();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            throw exp;
        }
    }

    #endregion

    #endregion

    #region 無法製卡檔查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:無法製卡檔查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0502Report(Dictionary<string, string> param, ref string strPathFile, ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0502Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int indexInSheetStart = 2;

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0502Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 換卡異動檔查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:換卡異動檔查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0503Report(Dictionary<string, string> param, ref string strPathFile, ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

            // 取要下載的資料

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text, CommandText = SearchExport0503
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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0503Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int indexInSheetStart = 2;

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);;


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0503Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 郵局退件資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵局退件資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0504Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

            // 取要下載的資料

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text, CommandText = SearchExport0504
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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0504Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Range range = null;
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int intRowIndexInSheet = 1;

            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 11];
            for (int intLoop = 0; intLoop < dtblSearchResult.Rows.Count; intLoop++)
            {
                intRowIndexInSheet++;

                // 退件流水號
                arrExportData[intLoop, 0] = dtblSearchResult.Rows[intLoop]["serial_no"].ToString();
                // 類別
                arrExportData[intLoop, 1] = dtblSearchResult.Rows[intLoop]["Kind"].ToString();
                // 卡別
                arrExportData[intLoop, 2] = dtblSearchResult.Rows[intLoop]["Action"].ToString();
                // 退件日期
                arrExportData[intLoop, 3] = dtblSearchResult.Rows[intLoop]["BackDate"].ToString();
                // 退件原因
                arrExportData[intLoop, 4] = dtblSearchResult.Rows[intLoop]["Reason"].ToString();
                // 卡號
                arrExportData[intLoop, 5] = dtblSearchResult.Rows[intLoop]["CardNo"].ToString();
                // 結案日期
                arrExportData[intLoop, 6] = dtblSearchResult.Rows[intLoop]["CloseDate"].ToString();
                // 處理方式
                arrExportData[intLoop, 7] = dtblSearchResult.Rows[intLoop]["EndItem"].ToString();
                // 郵寄日期
                arrExportData[intLoop, 8] = dtblSearchResult.Rows[intLoop]["MailDate"].ToString();
                // 掛號號碼
                arrExportData[intLoop, 9] = dtblSearchResult.Rows[intLoop]["MailNo"].ToString();
                // 經辦
                arrExportData[intLoop, 10] = dtblSearchResult.Rows[intLoop]["EnduId"].ToString();
            }

            // 賦予查詢結果
            range = sheet.Range["A2", "K" + intRowIndexInSheet];
            range.Value2 = arrExportData;

            // 設置樣式
            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0504Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 註銷作業報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:註銷作業報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0506Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

            // 取要下載的資料

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text, CommandText = SearchExport0506
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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0506Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Range range = null;
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int indexInSheetStart = 2;
            int indexInSheetEnd = indexInSheetStart;

            //統計
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

            #region 導入數據查詢結果

            range = sheet.Range["A" + indexInSheetStart, "D" + (indexInSheetEnd - 1)];
            range.Value2 = arrExportData;

            #endregion

            #region Excel Style 樣式

            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();

            #endregion

            #region 移轉範本頁尾至資料結果下方

            //sheet1 內容
            int pageFooterNum = indexInSheetEnd;
            Range range1 = sheet.Range["A" + pageFooterNum, "D" + pageFooterNum];
            //sheet2 頁尾
            Worksheet sheet2 = (Worksheet) workbook.Sheets[2];
            Range range2 = sheet2.Range["A1", "D3"];
            //合併
            range2.Copy();
            sheet.Paste(range1, false);
            //刪除 頁尾暫存
            sheet2.Delete();

            //計算成功筆數、失敗筆數、失敗筆數

            sheet.Cells.Replace("$SuccessNum$", successNum);
            sheet.Cells.Replace("$FailNum$", failNum);
            sheet.Cells.Replace("$TotalNum$", totalNum);

            #endregion


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0506Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 簡訊發送查詢報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:簡訊發送查詢報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0507Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

            // 取要下載的資料

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
            DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0507Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Range range = null;
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int indexInSheetStart = 2;
            int indexInSheetEnd = indexInSheetStart;

            //統計
            int totalNum = dtblSearchResult.Rows.Count;

            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 6];
            for (int intLoop = 0; intLoop < totalNum; intLoop++)
            {
                indexInSheetEnd++;

                // 身分證字號
                arrExportData[intLoop, 0] = dtblSearchResult.Rows[intLoop]["ID"].ToString();
                // 卡號
                arrExportData[intLoop, 1] = dtblSearchResult.Rows[intLoop]["CardNo"].ToString();
                // 匯入日期
                arrExportData[intLoop, 2] = dtblSearchResult.Rows[intLoop]["Imp_Date"].ToString();
                // 交寄日
                arrExportData[intLoop, 3] = dtblSearchResult.Rows[intLoop]["Maildate"].ToString();
                // 掛號號碼
                arrExportData[intLoop, 4] = dtblSearchResult.Rows[intLoop]["Mailno"].ToString();
                // 狀態
                arrExportData[intLoop, 5] = dtblSearchResult.Rows[intLoop]["Ams"].ToString();
            }

            #region 導入數據查詢結果

            range = sheet.Range["A" + indexInSheetStart, "F" + (indexInSheetEnd - 1)];
            range.Value2 = arrExportData;

            #endregion

            #region Excel Style 樣式

            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();

            #endregion

            #region 移轉範本頁尾至資料結果下方

            //sheet1 內容
            int pageFooterNum = indexInSheetEnd;
            Range range1 = sheet.Range["A" + pageFooterNum, "F" + pageFooterNum];

            #endregion


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0507Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            //Logging.Log(ex, LogLayer.BusinessRule);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 郵局寄送資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵局寄送資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0508Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

            // 取要下載的資料

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0508Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Range range = null;
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int indexInSheetStart = 2;
            int indexInSheetEnd = indexInSheetStart;

            //統計
            int totalNum = dtblSearchResult.Rows.Count;

            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 6];
            for (int intLoop = 0; intLoop < totalNum; intLoop++)
            {
                indexInSheetEnd++;

                // 身分證字號
                arrExportData[intLoop, 0] = dtblSearchResult.Rows[intLoop]["ID"].ToString();
                // 卡號
                arrExportData[intLoop, 1] = dtblSearchResult.Rows[intLoop]["CardNo"].ToString();
                // 匯入日期
                arrExportData[intLoop, 2] = dtblSearchResult.Rows[intLoop]["Imp_Date"].ToString();
                // 交寄日
                arrExportData[intLoop, 3] = dtblSearchResult.Rows[intLoop]["Maildate"].ToString();
                // 掛號號碼
                arrExportData[intLoop, 4] = dtblSearchResult.Rows[intLoop]["Mailno"].ToString();
                // 狀態
                arrExportData[intLoop, 5] = dtblSearchResult.Rows[intLoop]["Info1Name"].ToString();
            }

            #region 導入數據查詢結果

            range = sheet.Range["A" + indexInSheetStart, "F" + (indexInSheetEnd - 1)];
            range.Value2 = arrExportData;

            #endregion

            #region Excel Style 樣式

            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();

            #endregion

            #region 移轉範本頁尾至資料結果下方

            //sheet1 內容
            int pageFooterNum = indexInSheetEnd;
            Range range1 = sheet.Range["A" + pageFooterNum, "F" + pageFooterNum];

            #endregion


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0508Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            //Logging.Log(ex, LogLayer.BusinessRule);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 製卡相關資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:製卡相關資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0519Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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

            //* 查詢數據
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0519Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 2;

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0519Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 退件日報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:退件日報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0516Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0516Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 5;

            // 報表列印日
            sheet.Range["L2"].Value = "列印日期：" + DateTime.Now.ToString("yyyy/MM/dd");

            // 處理日期
            sheet.Range["A2"].Value = "處理日期：" + param["Operaction"];

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0516Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 退件原因統計表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:退件原因統計表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0517Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0517Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 4;

            // 報表列印日
            sheet.Range["G2"].Value = "列印日期：" + DateTime.Now.ToString("yyyy/MM/dd");

            // 統計日期
            sheet.Range["A2"].Value = "統計日期：" + param["MstatrDate"] + "~" + param["MendDate"];

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            // 總計
            sheet.Range["A" + (dt.Rows.Count + indexInSheetStart)].Value = "總計";
            sheet.Range["B" + (dt.Rows.Count + indexInSheetStart)].Value = dt.Rows.Count;
            string[] group = {"C", "D", "E", "F", "G"};
            
            // 總計公式
            foreach (string g in group)
            {
                sheet.Range[g + (dt.Rows.Count + indexInSheetStart)].Formula = "=SUM(" + g + indexInSheetStart + ":" + g + (indexInSheetStart + dt.Rows.Count - 1) + ")";
            }

            #region Excel Style 樣式

            Range range = null;
            range = sheet.Range["A" + (dt.Rows.Count + indexInSheetStart), "G" + (dt.Rows.Count + indexInSheetStart)];
            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Font.Bold = true;
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();
            range.EntireRow.AutoFit();

            #endregion


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0517Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 退件連絡報表 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:退件連絡報表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0518Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlSearchData = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SearchExport0518
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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0518Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 4;

            // 報表列印日
            sheet.Range["J2"].Value = "列印日期：" + DateTime.Now.ToString("yyyy/MM/dd");

            // 統計日期
            sheet.Range["A2"].Value = "統計日期：" + param["datefrom"] + "~" + param["dateto"];

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);
            
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0518Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 址更重寄異動記錄查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:址更重寄異動記錄查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0520Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0520Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 4;

            // 報表列印日
            sheet.Range["L2"].Value = "列印日期：" + DateTime.Now.ToString("yyyy/MM/dd");

            // 統計日期
            sheet.Range["A2"].Value = "統計日期：" + param["strUpdFrom"] + "~" + param["strUpdTo"];

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);
            
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0520Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 分行郵寄資料查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:分行郵寄資料查詢 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0509Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0509Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 2;

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);
            
            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0509Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 扣卡明細查詢 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:扣卡明細查詢 - Excel  
    /// 作    者:Ares Luke
    /// 創建時間:2020/06/23
    /// </summary>
    /// 
    public static bool CreateExcelFile_0510Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0510Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet)workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 2;

            // 轉入結果資料
            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0510Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 自取庫存日結 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:自取庫存日結 - Excel  
    /// 作    者:Ares Luke
    /// 創建時間:2020/06/29
    /// </summary>

    public static bool CreateExcelFile_0207Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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
            DataSet dstSearchData = SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0207Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet)workbook.Sheets[1];

            // 初始ROW位置
            int indexInSheetStart = 5;

            // 轉入結果資料
            //ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            #region Excel 依序塞資料

            // 總筆數
            int totalRowsNum = dt.Rows.Count;
            // 報表欄位筆數
            int totalColumnsNum = 4;

            int sumIntoStoreCount = 0;
            int sumOutStoreFCount = 0;
            int sumOutStoreMCount = 0;
            int sumOutStoreDCount = 0;
            int sumDailyCloseCount = 0;
            string dailyCloseDate = "";


            for (int intRowsLoop = 1; intRowsLoop <= totalRowsNum; intRowsLoop++)
            {
                for (int intColumnsLoop = 1; intColumnsLoop <= totalColumnsNum; intColumnsLoop++)
                {
                    sheet.Cells[intRowsLoop + (indexInSheetStart - 1), intColumnsLoop] = dt.Rows[intRowsLoop - 1][intColumnsLoop - 1];
                }

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

            // 報表列印日
            sheet.Cells.Replace("$PrintDate$", DateTime.Now.ToString("yyyy/MM/dd"));
            // 打印日期
            sheet.Cells.Replace("$DailyClose_Date$", dailyCloseDate);
            // 前日庫存
            sheet.Cells.Replace("$PreDailyCount$", preDailyCount);
            // 今日入庫
            sheet.Cells.Replace("$IntoStoreCount$", sumIntoStoreCount);
            // 今日領卡
            sheet.Cells.Replace("$OutStoreFCount$", sumOutStoreFCount);
            // 今日郵寄
            sheet.Cells.Replace("$OutStoreMCount$", sumOutStoreMCount);
            // 今日註銷
            sheet.Cells.Replace("$OutStoreDCount$", sumOutStoreDCount);
            // 今日庫存
            sheet.Cells.Replace("$DailyCloseCount$", sumDailyCloseCount);

            #endregion

            #region Excel Style 樣式

            Range range = null;
            range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[totalRowsNum + (indexInSheetStart - 1), totalColumnsNum]];
            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();
            range.EntireRow.AutoFit();

            #endregion

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0207Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 郵局查單申請處理 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:郵局查單申請處理 - Excel  
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/06
    /// </summary>

    public static bool CreateExcelFile_0401Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "0401Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Worksheet sheet = (Worksheet)workbook.Sheets[1];

            // 交寄日期年月日
            sheet.Cells.Replace("$year$", Convert.ToString(int.Parse(DateTime.Now.ToString("yyyy")) - 1911));
            sheet.Cells.Replace("$month$", DateTime.Now.ToString("MM"));
            sheet.Cells.Replace("$day$", DateTime.Now.ToString("dd"));
            // 住址
            sheet.Cells.Replace("$add1$", dt.Rows[0][dt.Columns["add1"]]);
            sheet.Cells.Replace("$add2$", dt.Rows[0][dt.Columns["add2"]]);
            sheet.Cells.Replace("$add3$", dt.Rows[0][dt.Columns["add3"]]);
            // 收件人姓名
            sheet.Cells.Replace("$custname$", dt.Rows[0][dt.Columns["custname"]]);
            //郵號件碼種類
            sheet.Cells.Replace("$mailno$", "'" + dt.Rows[0][dt.Columns["mailno"]].ToString());

            #endregion

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0401Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false, null, null);
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
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 卡片自取逾期明細表_1 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:卡片自取逾期明細表 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/08
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_020501Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "020501Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Range range = null;
            Worksheet sheet = (Worksheet)workbook.Sheets[1];

            //統計
            int totalNum = dt.Rows.Count;

            //初始ROW位置
            int indexInSheetStart = 5;
            int indexInSheetEnd = indexInSheetStart + totalNum;

            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            #region 移轉範本頁尾至資料結果下方

            //sheet1 內容
            int pageFooterNum = indexInSheetEnd;
            Range range1 = sheet.Range["A" + pageFooterNum, "K" + pageFooterNum];
            //sheet2 頁尾
            Worksheet sheet2 = (Worksheet)workbook.Sheets[2];
            Range range2 = sheet2.Range["A1", "K3"];
            //合併
            range2.Copy();
            sheet.Paste(range1, false);
            //刪除 頁尾暫存
            sheet2.Delete();

            //合計張數
            sheet.Cells.Replace("$totalCardNo$", totalNum);
            #endregion

            //自取時間
            sheet.Range["K2"].Value = "自取時間：" + DateTime.Now.ToString("yyyy/MM/dd");

            //列印時間
            sheet.Range["K3"].Value = "列印日期：" + param["strMerchDate"];


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "020501Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion

    #region 卡片自取逾期明細表_2 - Excel

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:卡片自取逾期明細表_2 - Excel 
    /// 作    者:Ares Luke
    /// 創建時間:2020/07/08
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_020502Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgId)
    {
        // 創建一個Excel實例
        ExcelApplication excel = new ExcelApplication();
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CheckDirectory(ref strPathFile);

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

            // 不顯示Excel文件，如果為true則顯示Excel文件
            excel.Visible = false;
            // 停用警告訊息
            excel.Application.DisplayAlerts = false;

            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory +
                                      ConfigurationManager.AppSettings["ReportTemplate"] + "020502Report.xlsx";
            Workbook workbook = excel.Workbooks.Open(strExcelPathFile);

            // 創建一個空的單元格對象
            Range range = null;
            Worksheet sheet = (Worksheet)workbook.Sheets[1];

            //統計
            int totalNum = dt.Rows.Count;

            //初始ROW位置
            int indexInSheetStart = 5;
            int indexInSheetEnd = indexInSheetStart + totalNum;

            ExportExcel(dt, ref sheet, indexInSheetStart - 1);

            #region 移轉範本頁尾至資料結果下方

            //sheet1 內容
            int pageFooterNum = indexInSheetEnd;
            Range range1 = sheet.Range["A" + pageFooterNum, "L" + pageFooterNum];
            //sheet2 頁尾
            Worksheet sheet2 = (Worksheet)workbook.Sheets[2];
            Range range2 = sheet2.Range["A1", "K3"];
            //合併
            range2.Copy();
            sheet.Paste(range1, false);
            //刪除 頁尾暫存
            sheet2.Delete();

            //合計
            sheet.Cells.Replace("$totalCardNo$", totalNum);
            #endregion

            //逾期時間
            sheet.Range["K2"].Value = "逾期時間：" + param["strFetchDate"];

            //列印時間
            sheet.Range["K3"].Value = "列印日期：" + DateTime.Now.ToString("yyyy/MM/dd");



            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "020502Report" + ".xlsx";
            sheet.SaveAs(strPathFile);

            // 關閉Excel文件且不保存
            excel.ActiveWorkbook.Close(false);
            return true;

            #endregion 匯入文檔結束
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw ex;
        }
        finally
        {
            // 退出 Excel
            excel.Quit();
            // 將 Excel 實例設置為空
            excel = null;
        }
    }

    #endregion


    #region 匯入EXCEL資料

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:共用匯入EXCEL資料 - Excel
    /// 作    者:Ares Luke
    /// 創建時間:2020/06/29
    /// </summary>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    private static void ExportExcel(DataTable dt, ref Worksheet sheet, int intRows)
    {
        // 總筆數
        int totalRowsNum = dt.Rows.Count;
        // 報表欄位筆數
        int totalColumnsNum = dt.Columns.Count;

        try
        {
            #region Excel 依序塞資料

            for (int intRowsLoop = 1; intRowsLoop <= totalRowsNum; intRowsLoop++)
            {
                for (int intColumnsLoop = 1; intColumnsLoop <= totalColumnsNum; intColumnsLoop++)
                {
                    sheet.Cells[intRowsLoop + intRows, intColumnsLoop] = dt.Rows[intRowsLoop - 1][intColumnsLoop - 1];
                }
            }

            #endregion

            #region Excel Style 樣式

            Range range = null;
            range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[totalRowsNum + intRows, totalColumnsNum]];
            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();
            range.EntireRow.AutoFit();

            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    #endregion
}