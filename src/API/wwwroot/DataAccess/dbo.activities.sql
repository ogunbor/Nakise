select c.Id, Title, c.Description, StartDate,EndDate, a.Type, a.Name, a.Id as ActivityId
from CallForApplications c
inner join Activities a 
on c.ActivityId = a.Id
where c.ProgrammeId = @programmeId

union all

select ass.Id, Title, ass.Description, StartDate,EndDate, a.Type, a.Name, a.Id as ActivityId
from Assessments ass
inner join Activities a 
on ass.ActivityId = a.Id
where ass.ProgrammeId = @programmeId

union all

select s.Id, Title, s.Description, StartDate,EndDate, a.Type, a.Name, a.Id as ActivityId
from Surveys s
inner join Activities a 
on s.ActivityId = a.Id
where s.ProgrammeId = @programmeId

union all

select f.Id, Title, f.Description, StartDate,EndDate, a.Type, a.Name, a.Id as ActivityId
from Forms f
inner join Activities a 
on f.ActivityId = a.Id
where f.ProgrammeId = @programmeId

union all

select e.Id, Title, e.Description, StartDate,EndDate, a.Type, a.Name, a.Id as ActivityId
from Events e
inner join Activities a 
on e.ActivityId = a.Id
where e.ProgrammeId = @programmeId

union all

select t.Id, Title, t.Description, StartDate,EndDate, a.Type, a.Name, a.Id as ActivityId
from Trainings t
inner join Activities a 
on t.ActivityId = a.Id
where t.ProgrammeId = @programmeId