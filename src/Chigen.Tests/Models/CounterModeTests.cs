using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class CounterModeTests
    {
        [Fact]
        public void CounterMode_HasPeripheralBlood()
        {
            Assert.Equal(0, (int)CounterMode.PeripheralBlood);
        }

        [Fact]
        public void CounterMode_HasBoneMarrow()
        {
            Assert.Equal(1, (int)CounterMode.BoneMarrow);
        }
    }
}
