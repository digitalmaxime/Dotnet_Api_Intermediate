using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNetDemo;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

var summary = BenchmarkRunner.Run<Md5VsSha256>();
