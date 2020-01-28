﻿using FluentAssertions;
using HealthChecks.UI;
using HealthChecks.UI.Core.Discovery.K8S;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace UnitTests.UI.Kubernetes
{
    public class kubernetes_address_factory_should
    {
        [Fact]
        public void parse_properly_the_k8s_api_discovered_services_for_a_local_cluster()
        {
            var healthPath = "healthz";
            var apiResponse = File.ReadAllText("UI/Kubernetes/SampleData/local-cluster-discovery-sample.json");

            var services = JsonConvert.DeserializeObject<ServiceList>(apiResponse);


            var addressFactory = new KubernetesAddressFactory(new KubernetesDiscoverySettings
            {
                HealthPath = healthPath,
                ServicesPathLabel = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_PATH_LABEL,
                ServicesPortLabel = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_PORT_LABEL,
                ServicesSchemeLabel = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_SCHEME_LABEL
            });

            List<string> serviceAddresses = new List<string>();
            
            foreach(var item in services.Items)
            {
                serviceAddresses.Add(addressFactory.CreateAddress(item));
            }

            serviceAddresses[0].Should().Be("http://localhost:10000/healthz");
            serviceAddresses[1].Should().Be("http://localhost:9000/healthz");
            serviceAddresses[2].Should().Be("http://localhost:30000/healthz");
            serviceAddresses[3].Should().Be("http://10.97.1.153:80/healthz");
            serviceAddresses[4].Should().Be("http://[2001:0db8:85a3:0000:0000:8a2e:0370:7334]:7070/custom/health/path");

        }

        [Fact]
        public void parse_properly_the_k8s_api_discovered_services_for_a_remote_cluster()
        {
            var healthPath = "healthz";
            var apiResponse = File.ReadAllText("UI/Kubernetes/SampleData/remote-cluster-discovery-sample.json");

            var services = JsonConvert.DeserializeObject<ServiceList>(apiResponse);

            var addressFactory = new KubernetesAddressFactory(new KubernetesDiscoverySettings
            {
                HealthPath = healthPath,
                ServicesPathAnnotation = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_PATH_LABEL,
                ServicesPortAnnotation = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_PORT_LABEL,
                ServicesSchemeAnnotation = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_SCHEME_LABEL
            });

            List<string> serviceAddresses = new List<string>();

            foreach (var item in services.Items)
            {
                serviceAddresses.Add(addressFactory.CreateAddress(item));
            }

            serviceAddresses[0].Should().Be("http://13.73.139.23:80/healthz");
            serviceAddresses[1].Should().Be("http://13.80.181.10:51000/healthz");
            serviceAddresses[2].Should().Be("http://12.0.0.190:5672/healthz");
            serviceAddresses[3].Should().Be("http://12.0.0.168:30478/healthz");
            serviceAddresses[4].Should().Be("https://10.152.183.35:8080/custom/health/path");

        }

    }
}
