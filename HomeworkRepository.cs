using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;

public class HomeworkRepository : BaseRepository, IHomework<Homework>
{
    public HomeworkRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<IEnumerable<Homework>> GetAll()
    {
        using var connection = CreateConnection();
        List<Homework> homework = (List<Homework>)await connection.QueryAsync<Homework>("SELECT * FROM homework;");

        for (int i = 0; i < homework.Count; i++)
        {
            homework[i].children = (List<Child>)await connection.QueryAsync<Child>(@"
                SELECT children.id, children.name, children.avatar, childrensHomework.image,
                childrensHomework.comment, childrensHomework.annotation
                FROM children
                FULL OUTER JOIN
                childrensHomework
                ON
                children.id = childrensHomework.childId
                WHERE homeworkId=@Id ORDER BY id;",
                new { Id = homework[i].Id });
        }
        return homework;
    }

    public async Task Insert(Homework homework)
    {
        using var connection = CreateConnection();

        // create homework record, returning just the id of the object
        long homeworkId = await connection.QuerySingleAsync<long>("INSERT INTO homework (Name, Image, Datedue, Comment) VALUES (@Name, @Image, @Datedue, @Comment) RETURNING id;", homework);

        // get the empty "classroom" from the children table
        List<Child> children = (List<Child>)await connection.QueryAsync<Child>(@"SELECT * FROM children;");

        foreach (Child child in children)
        {
            await connection.ExecuteAsync("INSERT INTO childrensHomework (homeworkid, childid) VALUES (@HomeworkId, @ChildId);",
            new { HomeworkId = homeworkId, ChildId = child.Id });
        }
    }

    // Update is for Mark and Reject
    public async Task<Homework> Update(long id, long childId, string image, string comment, string annotation)
    {
        using var connection = CreateConnection();

        Homework record = (Homework)await connection.QuerySingleAsync(@"
            UPDATE childrensHomework 
            SET image = @Image, comment = @Comment, annotation = @Annotation 
            WHERE homeworkid= @HomeworkId AND childid = @ChildId RETURNING *;",
            new
            {
                HomeworkId = id,
                ChildId = childId,
                Image = image,
                Comment = comment,
                Annotation = annotation
            });

        return record;
    }
}