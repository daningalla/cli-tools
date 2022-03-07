namespace Vertical.JsonClassGenerator;

public interface IJsonCallback<TState>
{
    TState CreateState();
    
    void VisitObject(string path, TState state);

    void VisitArray(string path, TState state);

    void VisitValue<T>(string path, T value, TState state) where T : notnull;

    void VisitNull(string path, TState state);

    void LeaveObject(string path, TState state);

    void LeaveArray(string path, TState state);
}