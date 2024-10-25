namespace IAndOthers.Core.Data.Entity
{
    public abstract class IOEntityDeletable : IOEntityTrackable
    {
        public bool Deleted { get; set; }
    }
}
