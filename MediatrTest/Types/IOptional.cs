using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatrTest.Types
{
    public interface IOptional<out TService>
    {
        TService Instance { get; }
    }

    public class Optional<TService> : IOptional<TService>
    {

        public Optional(IEnumerable<TService> services)
        {
            Instance = services.FirstOrDefault();
        }
        
        public TService Instance { get; }
    }
}