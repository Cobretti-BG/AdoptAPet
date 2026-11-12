
namespace AdoptAPet.Common
{
    public static class ServiceErrorMessages
    {
        public const string PetNotFound = "The requested pet could not be found.";
        public const string RequestNotFound = "The adoption request could not be found.";
        public const string Unauthorized = "You do not have permission to perform this action.";
        public const string UserNotAuthenticated = "You must be logged in to perform this action.";
        public const string InvalidRequestId = "The adoption request ID is invalid.";
        public const string ValidationFailed = "Please check the provided information and try again.";
        public const string CannotAdoptOwnPet = "You cannot apply to adopt your own pet.";
        public const string PetNotAvailable = "This pet is no longer available for adoption.";
        public const string UserNotFound = "The requested user could not be found.";
        public const string PetHasAdoptionRequests = "This pet has pending adoption requests and cannot be deleted.";
        public const string DuplicateRequest = "You already have a pending adoption request for this pet.";
        public const string InvalidPetId = "The pet ID is invalid.";
        public const string RequestNotPending = "This adoption request is no longer pending.";
        public const string PetInformationRequired = "Pet information is required.";
        public const string PetCannotBeUpdated = "Only available pet listings can be updated.";
        public const string AdoptionRequestInformationRequired = "Adoption request information is required.";
        public const string RequestCannotBeCancelled = "Only pending adoption requests can be cancelled.";
        public const string RequestCannotBeRejected = "Only pending adoption requests can be rejected.";
        public const string RequestCannotBeApproved = "Only pending adoption requests can be approved.";
    }
}
