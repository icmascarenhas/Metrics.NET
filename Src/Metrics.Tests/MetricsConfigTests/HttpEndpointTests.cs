﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Metrics;
using System.Net.NetworkInformation;
using System.Collections;
using System.Net;

namespace Metrics.Tests.MetricsConfigTests
{
    public class HttpEndpointTests
    {

        [Fact]
        public void HttpEndpointCanBeDisposed()
        {
            var config = Metric.Config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/HttpEndpointCanBeDisposed/");
            config.Dispose();
        }

        [Fact]
        public void HttpEndpointDoesNotThrowIfPortIsOccupied()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:58888/metricstest/HttpListenerTests/OccupiedPort/");
                listener.Start();
                
                Metric.Config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/OccupiedPort/");
                listener.Close();
            }
        }

        [Fact]
        public void HttpEndportLogsAnErrorIfPortIsOccupied()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:58888/metricstest/HttpListenerTests/OccupiedPort/");
                listener.Start();

                var loggedAnError = false;
                using (var config = Metric.Config)
                {
                    config.WithErrorHandler((exception, s) => { loggedAnError = true; }, true);
                    config.WithErrorHandler((exception) => { loggedAnError = true; }, true);

                    config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/OccupiedPort/");
                }
                Assert.True(loggedAnError);
                listener.Close();
            }
        }


        [Fact]
        public void SecondCallToWithHttpEndportDoesNotThrow()
        {
            using (var config = Metric.Config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/sameendpoint/"))
            {
                config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/sameendpoint/");
            }
        }

        [Fact]
        public void HttpEndpointDoesNotThrowOnDispose()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:58888/metricstest/HttpListenerTests/OccupiedPort/");
                listener.Start();

                var config = Metric.Config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/OccupiedPort/");
                config.Dispose();
                listener.Close();
            }
        }

        [Fact]
        public void NoErrorIsLoggedWhenHttpEndpointIsDisposed()
        {
            var loggedAnError = false;
            using (var config = Metric.Config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/DisposeMeOnce/"))
            {
                config.WithErrorHandler((exception, s) => { loggedAnError = true; Console.WriteLine(exception.ToString()); }, true);
                config.WithErrorHandler((exception) => { loggedAnError = true; Console.WriteLine(exception.ToString()); }, true);
            }
            
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            
            Assert.False(loggedAnError);
        }

        [Fact]
        public void CanSurviveDoubleDisposal()
        {
            using (var config = Metric.Config.WithHttpEndpoint("http://localhost:58888/metricstest/HttpListenerTests/DisposeMeTwice/"))
            {
                config.Dispose();
                Assert.DoesNotThrow(() => config.Dispose());
            }
        }

    }
}
