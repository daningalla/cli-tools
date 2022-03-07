using System.Text.Json;

namespace Vertical.JsonClassGenerator;

public class JsonReader : IAsyncDisposable
{
    private const int BufferSize = 32000;
    private readonly Stream _stream;
    private readonly bool _leaveOpen;

    public JsonReader(Stream stream, bool leaveOpen = false)
    {
        _stream = stream;
        _leaveOpen = leaveOpen;
    }

    public ValueTask DisposeAsync()
    {
        return _leaveOpen
            ? ValueTask.CompletedTask 
            : _stream.DisposeAsync();
    }

    public async Task ReadAsync<TState>(IJsonCallback<TState> jsonCallback)
    {
        await using var memoryStream = new MemoryStream(BufferSize);
        await _stream.CopyToAsync(memoryStream);
        ReadJsonBytesToCallback(jsonCallback, jsonCallback.CreateState(), memoryStream.ToArray());
    }

    private void ReadJsonBytesToCallback<TState>(
        IJsonCallback<TState> callback,
        TState state,
        byte[] jsonBytes)
    {
        ReadJsonToCallback(callback, state, new Utf8JsonReader(jsonBytes));
    }

    private void ReadJsonToCallback<TState>(
        IJsonCallback<TState> callback,
        TState state,
        Utf8JsonReader jsonReader)
    {
        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    VisitRootObject(callback, state, ref jsonReader);
                    break;
                
                case JsonTokenType.EndObject:
                    return;
            }
        }
    }

    private void VisitRootObject<TState>(
        IJsonCallback<TState> callback,
        TState state,
        ref Utf8JsonReader jsonReader)
    {
        VisitObject(callback, state, JsonPath.Root, ref jsonReader);
    }

    private void VisitObject<TState>(
        IJsonCallback<TState> callback, 
        TState state, 
        string path, 
        ref Utf8JsonReader jsonReader)
    {
        callback.VisitObject(path, state);

        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    var name = jsonReader.GetString() ?? throw Exceptions.NoPropertyName(path, jsonReader);
                    VisitProperty(callback, state, JsonPath.Combine(path, name), ref jsonReader);
                    break;
                
                case JsonTokenType.EndObject:
                    callback.LeaveObject(path, state);
                    return;
            }
        }

        throw Exceptions.NoEndObject(path, jsonReader);
    }
    
    private void VisitArray<TState>(
        IJsonCallback<TState> callback, 
        TState state, 
        string path, 
        ref Utf8JsonReader jsonReader)
    {
        callback.VisitArray(path, state);

        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    VisitObject(callback, state, JsonPath.CombineElementName(path), ref jsonReader);
                    break;
                
                case JsonTokenType.StartArray:
                    VisitArray(callback, state, JsonPath.CombineElementName(path), ref jsonReader);
                    break;
                
                case JsonTokenType.EndArray:
                    callback.LeaveArray(path, state);
                    return;
                
                default:
                    VisitValue(callback, state, JsonPath.CombineElementName(path), ref jsonReader);
                    break;
            }
        }
    }

    private void VisitProperty<TState>(
        IJsonCallback<TState> callback, 
        TState state, 
        string path, 
        ref Utf8JsonReader jsonReader)
    {
        if (jsonReader.Read())
        {
            VisitValue(callback, state, path, ref jsonReader);
        }
    }
    
    private void VisitValue<TState>(
        IJsonCallback<TState> callback, 
        TState state, 
        string path, 
        ref Utf8JsonReader jsonReader)
    {
        do
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                    callback.VisitValue(path, jsonReader.GetBoolean(), state);
                    return;

                case JsonTokenType.String:
                    callback.VisitValue(path, jsonReader.GetString()!, state);
                    return;

                case JsonTokenType.Number when jsonReader.TryGetInt32(out var value):
                    callback.VisitValue(path, value, state);
                    return;

                case JsonTokenType.Number when jsonReader.TryGetInt64(out var value):
                    callback.VisitValue(path, value, state);
                    return;

                case JsonTokenType.Number when jsonReader.TryGetSingle(out var value):
                    callback.VisitValue(path, value, state);
                    return;

                case JsonTokenType.Number when jsonReader.TryGetDouble(out var value):
                    callback.VisitValue(path, value, state);
                    return;

                case JsonTokenType.Number when jsonReader.TryGetDecimal(out var value):
                    callback.VisitValue(path, value, state);
                    return;

                case JsonTokenType.Null:
                    callback.VisitNull(path, state);
                    return;

                case JsonTokenType.StartObject:
                    VisitObject(callback, state, path, ref jsonReader);
                    return;

                case JsonTokenType.StartArray:
                    VisitArray(callback, state, path, ref jsonReader);
                    return;
            }
        } while (jsonReader.Read());
    }
}