using System;
using System.Diagnostics;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace SharpLimiter_Tests;

public class UnitTest1
{
    private const long _DELTA = 5;
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void Precision()
    {
        var _limmiter = new SharpLimiter.SharpLimiter(1, TimeSpan.FromMilliseconds(100));
        var sw = new Stopwatch();
        for (var i = 0; i < 50; i++)
        {
            await   _limmiter.ExecuteOnLimiter();
            sw.Restart();
            await   _limmiter.ExecuteOnLimiter();
            sw.ElapsedMilliseconds.Should().BeInRange(100 - _DELTA, 100 + _DELTA, "Limiter implementation should be precise enough");
        }
        _limmiter.Dispose();
    }

    [Fact]
    public async void InitialValue()
    {
        
        var _limmiter = new SharpLimiter.SharpLimiter(1, TimeSpan.FromMilliseconds(10000), 5);
        var sw = new Stopwatch();

        for (var i = 0; i < 4; i++)
        {
            sw.Restart();
            await   _limmiter.ExecuteOnLimiter();
            sw.ElapsedMilliseconds.Should().BeInRange(0, _DELTA , "Limiter implementation should be precise enough");
        }
        _limmiter.Dispose();
    }
}