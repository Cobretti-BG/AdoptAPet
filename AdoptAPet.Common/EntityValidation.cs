namespace AdoptAPet.Common
{
    public static class EntityValidation
    {
        public static class User
        {
            public const int UserNameMinLength = 3;
            public const int UserNameMaxLength = 30;
        }

        public static class Pet
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;

            public const int SpeciesMinLength = 2;
            public const int SpeciesMaxLength = 30;

            public const int BreedMaxLength = 50;
            public const int ColorMaxLength = 30;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 2000;

            public const int LocationMinLength = 2;
            public const int LocationMaxLength = 100;

            public const int ImageUrlMaxLength = 2048;
        }

        public static class AdoptionRequest
        {
            public const int MessageMinLength = 10;
            public const int MessageMaxLength = 1000;
        }
    }
}
