namespace CmdBased
{
    public delegate void Procedure();
    public delegate void EndProcedure(bool interrupted);
    public delegate bool Predicate();
}