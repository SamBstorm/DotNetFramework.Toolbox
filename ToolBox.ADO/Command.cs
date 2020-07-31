﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.ADO
{
    public class Command
    {
        public string SqlQuery { get; private set; }

        public bool IsStoredProcedure { get; private set; }

        public Dictionary<string, SqlParameter> Parameters { get; private set; }

        public Command(string sqlQuery, bool isStoredProcedure = false)
        {
            SqlQuery = sqlQuery;
            IsStoredProcedure = isStoredProcedure;
            Parameters = new Dictionary<string, SqlParameter>();
        }

        public void AddParameter(string parameterName, object value, bool isOutput = false)
        {
            if (Parameters.TryGetValue(parameterName, out _)) throw new ArgumentException("Paramètre déjà présent.", nameof(parameterName));
            SqlParameter param = new SqlParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            param.Direction = (isOutput) ? System.Data.ParameterDirection.Output : System.Data.ParameterDirection.Input;
            Parameters.Add(parameterName, param);
        }
    }
}
