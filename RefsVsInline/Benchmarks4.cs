using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InlineVsRef;
public ref struct ValueRef {

    public ValueRef(in int _value, in Span<int> _span) {
        Value = _value;
        span = _span;
    }

    public int Value;
    public Span<int> span;
}

public struct ValueStruct {
    public ValueStruct(in int _value, in int[] _span) {
        Value = _value;
        span = _span;
    }
    public int Value;
    public int[] span;
}

[MemoryDiagnoser]
public class RefsInlinedVsNot
{
    
    [Benchmark]
    public void PassRefStructByValueInlined() {
        ValueRef value = new ValueRef(1, new int[1]);
        passByValueInlined(value);
    }
    
    [Benchmark]
    public void PassRefStructByValueNotInlined() {
        ValueRef value = new ValueRef(1, new int[1]);
        passByValueStandard(value);
    }

    [Benchmark]
    public void PassRefStructByRef() {
        ValueRef value = new ValueRef(1, new int[1]);
        passByRef(ref value);
    }

    [Benchmark]
    public void PassNormalStructByValueInlined() {
        ValueStruct value = new ValueStruct(1, new int[1]);
        passByValueInlined(value);
    }

    [Benchmark]
    public void PassNormalStructByValueNotInlined() {
        ValueStruct value = new ValueStruct(1, new int[1]);
        passByValueStandard(value);
    }

    [Benchmark]
    public void PassNormalStructByRef() {
        ValueStruct value = new ValueStruct(1, new int[1]);
        passByRef(ref value);
    }


    public void passByValueStandard(ValueRef value) {
        value.Value = 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void passByValueInlined(ValueRef value) {
        value.Value = 1;
    }
    
    public void passByValueStandard(ValueStruct value) {
        value.Value = 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void passByValueInlined(ValueStruct value) {
        value.Value = 1;
    }

    public void passByRef(ref ValueRef value) {
        value.Value = 1;
    }

    public void passByRef(ref ValueStruct value) {
        value.Value = 1;
    }
    
}