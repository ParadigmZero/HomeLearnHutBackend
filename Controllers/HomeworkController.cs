using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class HomeworkController : ControllerBase
{
    private readonly IHomework<Homework> _homeworkRepository;

    public HomeworkController(IHomework<Homework> homeworkRepository)
    {
        _homeworkRepository = homeworkRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var homework = await _homeworkRepository.GetAll();
            return Ok(homework);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] Homework homework)
    {
        try
        {
            await _homeworkRepository.Insert(homework);
            return await GetAll();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] Child child)
    {
        try
        {
            await _homeworkRepository.Update(id, child.Id, child.Image, child.Comment, child.Annotation);
            return await GetAll();
        }
        catch
        {
            return BadRequest();
        }
    }
}