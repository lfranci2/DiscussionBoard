namespace DiscussionBoard.ViewModels
{
    public class UserProfile
    {
        public string DisplayName { get; set; }
        public string AvatarId { get; set; }

        public UserProfile()
        {
        }

        public UserProfile(string displayName, string avatarId)
        {
            DisplayName = displayName;
            AvatarId = avatarId;
        }
    }
}
