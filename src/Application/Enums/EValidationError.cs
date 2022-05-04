namespace Application.Enums
{
    public enum EValidationError
    {
        EnterValue = 1,
        EnterEmailValue = 2,
        EnterStrongPassword = 3,
        EnterValidValue = 4,
        UnAuthorized = 5,
        AgeRangeCheck = 6,
        DateRangeCheck = 7,
        NotMoreThan4MB,
        CheckFileFormat,
        MaxLengthIs100,
        FieldOptionsEmpty,
        DuplicateFieldIndex,
        StageMustBeEmpty,
        StageNotEmpty,
        StageMustContainStatuses,
        FileEmpty,
        FileShouldBeEmpty,
        ValueShouldBeEmpty
    }
}
