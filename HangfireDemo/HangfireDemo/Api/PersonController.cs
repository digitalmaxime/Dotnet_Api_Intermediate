using Microsoft.AspNetCore.Mvc;
using System.Net;
using Hangfire;
using HangfireDemo.Domain;
using HangfireDemo.Infra;

namespace HangfireDemo.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v1")]
public class PersonController : ControllerBase
{
    private readonly IPersonRepository _personRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<PersonController> _logger;

    public PersonController(IPersonRepository personRepository, IBackgroundJobClient backgroundJobClient, ILogger<PersonController> logger)
    {
        _personRepository = personRepository;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("person/{personId}")]
    [ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("GetStudentById/{personId}")]
    public async Task<ActionResult<Person>> GetStudentById(int personId)
    {
        var res = await _personRepository.GetPersonById(personId);
        
        if (res == null)
        {
            return NotFound($"not found with person id {personId}");
        }
        
        return Ok(res);
    }

    [HttpPost]
    [Route("person")]
    [ProducesResponseType(typeof(Person), (int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Person(PersonDto personDto)
    {
        var person = new Person { Name = personDto.Name };
        var jobId = _backgroundJobClient.Enqueue<IPersonRepository>(repo => repo.CreatePerson(person));
        // _backgroundJobClient.Enqueue(() => _personRepository.CreatePerson(person)); // Equivalent!
        
        // Job Continuation 
        _backgroundJobClient.ContinueJobWith(jobId, () => Console.WriteLine($"The job with jobId {jobId} has finished"));

        
        return Created("api/v1/person", new {});
    }
    
    [HttpPost]
    [Route("schedule")]
    [ProducesResponseType(typeof(Person), (int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Schedule(PersonDto personDto)
    {
        var person = new Person { Name = personDto.Name };
        _backgroundJobClient.Schedule<IPersonRepository>(repo => repo.CreatePerson(person), TimeSpan.FromSeconds(30));
        
        return Created("api/v1/schedule", new {});
    }

}