select 
concat('=HYPERLINK("https://doekodev.azurewebsites.net/Contracts/Details/',i.contractid,'",',i.contractid,')'),
concat('=HYPERLINK("https://doekodev.azurewebsites.net/Investments/Details/',s.investmentid,'","',s.investmentid,'")'),
concat('=HYPERLINK("https://doekodev.azurewebsites.net/Surveys/Maintain/',r.surveyid,'","',r.surveyid,'")'),
CONCAT('=VALUE("',r.finalrsepower,'")'), 
CONCAT('=VALUE("',r.finalpvconfig,'")'),  
CONCAT('=VALUE("',(r.finalrsepower / 0.28),'")') as 'calculated' 
from surveyresultcalculations as r
inner join survey as s
on s.surveyid = r.surveyid
inner join investment as i
on s.investmentid = i.investmentid
where s.rsetype = 5
and r.finalpvconfig <> (r.finalrsepower / 0.28);