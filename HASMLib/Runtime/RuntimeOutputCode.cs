namespace HASMLib.Runtime
{
    public enum RuntimeOutputCode
    {
        UnknownExpressionReference,
        UnknownConstantReference,
        UnknownVariableReference,
        StackOverFlow,
        OK,
        ExpectedIntegerVariable,
        ExpectedOtherType,
        ArgumentsAreExpected,
        ReturnInConstructorsAreNotAllowed,
        ObjectStackIsEmpty,
        DifferentClasses,
        ClassTypeExpected,
        DifferentTypes,
    }
}