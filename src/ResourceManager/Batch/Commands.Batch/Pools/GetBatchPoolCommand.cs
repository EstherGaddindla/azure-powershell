﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Batch;
using Microsoft.Azure.Commands.Batch.Models;
using Microsoft.Azure.Commands.Batch.Properties;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using Constants = Microsoft.Azure.Commands.Batch.Utils.Constants;

namespace Microsoft.Azure.Commands.Batch
{
    [Cmdlet(VerbsCommon.Get, "AzureBatchPool", DefaultParameterSetName = Constants.ODataFilterParameterSet), OutputType(typeof(PSCloudPool))]
    public class GetBatchPoolCommand : BatchObjectModelCmdletBase
    {
        private int maxCount = Constants.DefaultMaxCount;

        [Parameter(Position = 0, ParameterSetName = Constants.NameParameterSet, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the pool to retrieve.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = Constants.ODataFilterParameterSet, HelpMessage = "The OData filter clause to use when querying for pools.")]
        [ValidateNotNullOrEmpty]
        public string Filter { get; set; }

        [Parameter(ParameterSetName = Constants.ODataFilterParameterSet, HelpMessage = "The maximum number of pools to return.")]
        public int MaxCount
        {
            get { return this.maxCount; }
            set { this.maxCount = value; }
        }

        public override void ExecuteCmdlet()
        {
            ListPoolOptions options = new ListPoolOptions()
            {
                Context = this.BatchContext,
                PoolName = this.Name,
                Filter = this.Filter,
                MaxCount = this.MaxCount,
                AdditionalBehaviors = this.AdditionalBehaviors
            };

            // The enumerator will internally query the service in chunks. Using WriteObject with the enumerate flag will enumerate
            // the entire collection first and then write the items out one by one in a single group.  Using foreach, we can take 
            // advantage of the enumerator's behavior and write output to the pipeline in bursts.
            foreach (PSCloudPool pool in BatchClient.ListPools(options))
            {
                WriteObject(pool);
            }
        }
    }
}
