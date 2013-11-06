using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plexi.Models;

namespace Plexi.Interfaces
{
    public interface IPlexiService
    {
        string OriginalQuery { get; }

        void ClearContext();

        void ResetTimer();

        void Query(string query);

        void Choice(ChoiceModel choice);

        event EventHandler onShow;

        event EventHandler onAct;

        event EventHandler onChoice;

        event EventHandler onError;

        event EventHandler onProgress;
    }
}
