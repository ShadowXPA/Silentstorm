using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class ProjectStatus : IEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public static class Status
        {
            public const string Created = "CREATED";
            public const string Submission = "SUBMISSION";
            public const string Voting = "VOTING";
            public const string Developing = "DEVELOPING";
            public const string Finished = "FINISHED";
        }
    }
}
