﻿using CSIPCommonModel.BusinessRules;
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
    #region SQL語句

    #region SEARCH_Export_0502

    public const string SEARCH_Export_0502 = @"
SELECT
	u.ImportDate,
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
		'成功' ELSE '' 
	END AS outPutFlg,
CASE
		b.Merch_Code 
		WHEN 'A' THEN
		'宏通' 
		WHEN 'B' THEN
		'台銘' 
		WHEN 'C' THEN
		'金雅拓' 
	END AS Merch_NAME 
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

    #region SEARCH_Export_0503

    public const string SEARCH_Export_0503 = @"
SELECT
	BlockCode,
	ImportDate,
	CardNo,
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

    #region SEARCH_Export_0504

    public const string SEARCH_Export_0504 = @"
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

    #region SEARCH_Export_0506

    public const string SEARCH_Export_0506 = @"
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

    #region SEARCH_Export_0507

    public const string SEARCH_Export_0507 = @"
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

    #region SEARCH_Export_0508

    public const string SEARCH_Export_0508 = @"
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

    #region SEARCH_Export_0519

    public const string SEARCH_Export_0519 = @"
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

    #region SEARCH_Export_0516

    public const string SEARCH_Export_0516 = @"
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
    
    #region SEARCH_Export_0517

    public const string SEARCH_Export_0517 = @"
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
    
    #region SEARCH_Export_0518

    public const string SEARCH_Export_0518 = @"
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
    
    #region SEARCH_Export_0518

    public const string SEARCH_Export_0520 = @"
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
                if (strDirectories[intLoop].ToString() != strPath)
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
            //Logging.Log(exp, LogLayer.BusinessRule);
            throw exp;
        }
    }

    #endregion

    #endregion

    #region 無法製卡檔查詢 - Excel

    /// <summary>
    /// 無法製卡檔查詢 - Excel 
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="outFlg"></param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0502Report(string startDate, string endDate, string outFlg, string factory,
        ref string strPathFile, ref string strMsgID)
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
                CommandType = CommandType.Text, CommandText = SEARCH_Export_0502
            };

            SqlParameter paramStartDate = new SqlParameter("@startDate", startDate);
            sqlSearchData.Parameters.Add(paramStartDate);
            SqlParameter paramEndDate = new SqlParameter("@endDate", endDate);
            sqlSearchData.Parameters.Add(paramEndDate);
            SqlParameter paramOutFlg = new SqlParameter("@outFlg", outFlg);
            sqlSearchData.Parameters.Add(paramOutFlg);
            SqlParameter paramFactory = new SqlParameter("@factory", factory);
            sqlSearchData.Parameters.Add(paramFactory);

            //* 查詢數據
            DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgID = "06_05020000_004";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgID = "06_05020000_004";
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
            Range range = null;
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int intRowIndexInSheet = 1;

            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 8];
            for (int intLoop = 0; intLoop < dtblSearchResult.Rows.Count; intLoop++)
            {
                intRowIndexInSheet++;

                // 匯入日期
                arrExportData[intLoop, 0] = dtblSearchResult.Rows[intLoop]["ImportDate"].ToString();
                // 廠商
                arrExportData[intLoop, 1] = dtblSearchResult.Rows[intLoop]["Merch_NAME"].ToString();
                // 製卡日
                arrExportData[intLoop, 2] = dtblSearchResult.Rows[intLoop]["indate1"].ToString();
                // 身分證字號
                arrExportData[intLoop, 3] = dtblSearchResult.Rows[intLoop]["id"].ToString();
                // 姓名
                arrExportData[intLoop, 4] = dtblSearchResult.Rows[intLoop]["CustName"].ToString();
                // 卡號
                arrExportData[intLoop, 5] = dtblSearchResult.Rows[intLoop]["CardNo"].ToString();
                // 無法製出原因
                arrExportData[intLoop, 6] = dtblSearchResult.Rows[intLoop]["blockcode"].ToString();
                // 狀態
                arrExportData[intLoop, 7] = dtblSearchResult.Rows[intLoop]["outPutFlg"].ToString();
            }

            // 賦予查詢結果
            range = sheet.get_Range("A2", "H" + intRowIndexInSheet);
            range.Value2 = arrExportData;

            // 設置樣式
            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();


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

    #region 換卡異動檔查詢 - Excel

    /// <summary>
    /// 換卡異動檔查詢 - Excel 
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="outFlg"></param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0503Report(string startDate, string endDate, string outFlg,
        ref string strPathFile, ref string strMsgID)
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
                CommandType = CommandType.Text, CommandText = SEARCH_Export_0503
            };

            SqlParameter paramStartDate = new SqlParameter("@startDate", startDate);
            sqlSearchData.Parameters.Add(paramStartDate);
            SqlParameter paramEndDate = new SqlParameter("@endDate", endDate);
            sqlSearchData.Parameters.Add(paramEndDate);
            SqlParameter paramOutFlg = new SqlParameter("@outFlg", outFlg);
            sqlSearchData.Parameters.Add(paramOutFlg);

            //* 查詢數據
            DataSet dstSearchData = BR_Excel_File.SearchOnDataSet(sqlSearchData);

            #endregion 依據Request查詢資料庫

            #region 查無資料

            if (null == dstSearchData)
            {
                strMsgID = "06_05030000_004";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgID = "06_05030000_004";
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
            Range range = null;
            Worksheet sheet = (Worksheet) workbook.Sheets[1];

            //初始ROW位置
            int intRowIndexInSheet = 1;

            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 4];
            for (int intLoop = 0; intLoop < dtblSearchResult.Rows.Count; intLoop++)
            {
                intRowIndexInSheet++;

                // 匯入日期
                arrExportData[intLoop, 0] = dtblSearchResult.Rows[intLoop]["ImportDate"].ToString();
                // 卡號
                arrExportData[intLoop, 1] = dtblSearchResult.Rows[intLoop]["CardNo"].ToString();
                // 無法製出原因
                arrExportData[intLoop, 2] = dtblSearchResult.Rows[intLoop]["blockcode"].ToString();
                // 狀態
                arrExportData[intLoop, 3] = dtblSearchResult.Rows[intLoop]["outPutFlg"].ToString();
            }

            // 賦予查詢結果
            range = sheet.get_Range("A2", "D" + intRowIndexInSheet);
            range.Value2 = arrExportData;

            // 設置樣式
            range.Font.Size = 12;
            range.Font.Name = "新細明體";
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0503Report" + ".xlsx";
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

    #region 郵局退件資料查詢 - Excel

    /// <summary>
    /// 郵局退件資料查詢 - Excel 
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0504Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgID)
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
                CommandType = CommandType.Text, CommandText = SEARCH_Export_0504
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
                strMsgID = "06_06050400_004";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgID = "06_06050400_004";
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
            range = sheet.get_Range("A2", "K" + intRowIndexInSheet);
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

    #region 註銷作業報表 - Excel

    /// <summary>
    /// 註銷作業報表 - Excel 
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0506Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgID)
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
                CommandType = CommandType.Text, CommandText = SEARCH_Export_0506
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
                strMsgID = "06_06050600_000";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgID = "06_06050600_000";
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
            int TotalNum = dtblSearchResult.Rows.Count;


            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 4];
            for (int intLoop = 0; intLoop < TotalNum; intLoop++)
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

            range = sheet.get_Range("A" + indexInSheetStart, "D" + (indexInSheetEnd - 1));
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
            sheet.Cells.Replace("$TotalNum$", TotalNum);

            #endregion


            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "0506Report" + ".xlsx";
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

    #region 簡訊發送查詢報表 - Excel

    /// <summary>
    /// 簡訊發送查詢報表 - Excel 
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0507Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgID)
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
                CommandText = SEARCH_Export_0507
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
                strMsgID = "06_06050700_000";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgID = "06_06050700_000";
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
            int successNum = 0;
            int failNum = 0;
            int TotalNum = dtblSearchResult.Rows.Count;


            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 6];
            for (int intLoop = 0; intLoop < TotalNum; intLoop++)
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

            range = sheet.get_Range("A" + indexInSheetStart, "F" + (indexInSheetEnd - 1));
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
    /// 郵局寄送資料查詢 - Excel 
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgId">返回消息ID</param>
    /// <param name="param">查詢條件</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool CreateExcelFile_0508Report(Dictionary<string, string> param, ref string strPathFile,
        ref string strMsgID)
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
                CommandText = SEARCH_Export_0508
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
                strMsgID = "06_06050800_000";
                return false;
            }

            DataTable dtblSearchResult = dstSearchData.Tables[0];

            if (dtblSearchResult.Rows.Count == 0)
            {
                strMsgID = "06_06050800_000";
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
            int TotalNum = dtblSearchResult.Rows.Count;

            string[,] arrExportData = new string[dtblSearchResult.Rows.Count, 6];
            for (int intLoop = 0; intLoop < TotalNum; intLoop++)
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

            range = sheet.get_Range("A" + indexInSheetStart, "F" + (indexInSheetEnd - 1));
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
    /// 製卡相關資料查詢 - Excel 
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
                CommandText = SEARCH_Export_0519
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
    /// 退件日報表 - Excel 
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
                CommandText = SEARCH_Export_0516
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
    /// 退件原因統計表 - Excel 
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
                CommandText = SEARCH_Export_0517
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
    /// 退件連絡報表 - Excel 
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
                CommandText = SEARCH_Export_0518
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
    /// 址更重寄異動記錄查詢 - Excel 
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
                CommandText = SEARCH_Export_0520
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
    #region 匯入EXCEL資料

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