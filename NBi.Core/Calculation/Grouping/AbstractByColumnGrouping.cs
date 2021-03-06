﻿using NBi.Core.ResultSet;
using NBi.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Calculation.Grouping
{
    public abstract class AbstractByColumnGrouping : IByColumnGrouping
    {
        protected ISettingsResultSet Settings { get; }

        public AbstractByColumnGrouping(ISettingsResultSet settings)
        {
            Settings = settings;
        }

        public IDictionary<KeyCollection, DataTable> Execute(ResultSet.ResultSet resultSet)
        {
            var stopWatch = new Stopwatch();
            var dico = new Dictionary<KeyCollection, DataTable>();
            var keyComparer = BuildDataRowsKeyComparer(resultSet.Table);

            stopWatch.Start();
            foreach (DataRow row in resultSet.Rows)
            {
                var key = keyComparer.GetKeys(row);
                if (!dico.ContainsKey(key))
                    dico.Add(key, row.Table.Clone());
                dico[key].ImportRow(row);
            }
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Building rows' groups: {0} [{1}]", dico.Count, stopWatch.Elapsed.ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));

            return dico;
        }

        protected abstract DataRowKeysComparer BuildDataRowsKeyComparer(DataTable x);
    }
}
