select  
t2.UserName,
t1.Status, 
count(t1.SurveyId)
from dbo.SurveyStatusHistory as t1
left outer join dbo.AspNetUsers as t2
on t1.UserId = t2.Id
left outer join dbo.Survey as t3
on t1.Status = 5
and t1.SurveyId = t3.SurveyId
where t1.[End] = '9999-12-31T23:59:59.9999999'
and t1.Start >= '2018-01-01T00:00:00.0000000'
and t1.Start <= '2018-01-31T23:59:59.9999999'
and ( t3.CancelType IS NULL OR t3.CancelType = '2' )
group by t2.UserName, t1.Status
having t1.Status = '2' OR t1.status = 5
order by t2.UserName, t1.Status