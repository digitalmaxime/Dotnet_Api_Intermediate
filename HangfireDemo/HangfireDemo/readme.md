# Hangfire

### Simple Job (Enqueue):

`var jobId = _backgroundJobClient.Enqueue(() => _personRepository.CreatePerson(person));`

Even DI the service to be used by the Enqueue:

`_backgroundJobClient.Enqueue<IPersonRepository>(repo => repo.CreatePerson(person));`

---
### Schedule : 

`_backgroundJobClient.Schedule<IPersonRepository>(repo => repo.CreatePerson(person), TimeSpan.FromSeconds(30));`

---
### Recurring :

In programs.cs, after the `var app = builder.Build();` :

`RecurringJob.AddOrUpdate("easyjob", () => Console.Write("Easy!"), Cron.Minutely);`

---
### Continuations

chaining Jobs.
Running a Job returns a jobId;

Get the Id (string) of the job

backgroundJobClient.ContinueJobWith(jobId, () => Console.WriteLine($"The job {jobId} has finished")

---
### Deleting a job

You can do a query to the hangfire.job table to delete it

---
ref : https://www.youtube.com/watch?v=OkpXpMBUG9c&ab_channel=gavilanch3