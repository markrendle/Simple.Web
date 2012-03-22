namespace Simple.Web
{
    public interface IInput<in TInput>
    {
        TInput Input { set; }
    }
}