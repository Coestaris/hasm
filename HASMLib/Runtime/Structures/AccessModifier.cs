namespace HASMLib.Runtime.Structures
{
    public enum AccessModifier
    {
        Private,
        //Доступ только в текущем контексте

        Inner,
        //Доступ только в текущем контексте или дочерных классах

        Default,
        //Публичный доступ в пределах сборки

        Public,
        //Публичный доступ
    }
}
