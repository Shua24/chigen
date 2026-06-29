using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class DocumentTemplateTests
    {
        [Fact]
        public void DocumentTemplate_DefaultValues()
        {
            var t = new DocumentTemplate();
            Assert.Equal("Default", t.Name);
            Assert.Equal("", t.HeaderFormat);
            Assert.True(t.ShowLetterhead);
            Assert.True(t.ShowPatientInfo);
            Assert.True(t.ShowPatientId);
            Assert.True(t.ShowPatientName);
            Assert.True(t.ShowPatientDob);
            Assert.True(t.ShowPatientSex);
            Assert.True(t.ShowPatientDiagnosis);
            Assert.True(t.ShowPatientAddress);
            Assert.True(t.ShowPatientPhysician);
            Assert.True(t.ShowPatientWard);
            Assert.True(t.ShowPatientPaymentMethod);
            Assert.True(t.ShowReferenceRanges);
            Assert.True(t.ShowConclusion);
            Assert.True(t.ShowFooter);
        }

        [Fact]
        public void DocumentTemplate_SetProperties()
        {
            var t = new DocumentTemplate
            {
                Name = "Compact",
                HeaderFormat = "Custom",
                ShowLetterhead = false,
                ShowPatientInfo = false,
                ShowPatientId = false,
                ShowPatientName = false,
                ShowPatientDob = false,
                ShowPatientSex = false,
                ShowPatientDiagnosis = false,
                ShowPatientAddress = false,
                ShowPatientPhysician = false,
                ShowPatientWard = false,
                ShowPatientPaymentMethod = false,
                ShowReferenceRanges = false,
                ShowConclusion = false,
                ShowFooter = false
            };
            Assert.Equal("Compact", t.Name);
            Assert.Equal("Custom", t.HeaderFormat);
            Assert.False(t.ShowLetterhead);
            Assert.False(t.ShowPatientInfo);
            Assert.False(t.ShowPatientId);
            Assert.False(t.ShowPatientName);
            Assert.False(t.ShowPatientDob);
            Assert.False(t.ShowPatientSex);
            Assert.False(t.ShowPatientDiagnosis);
            Assert.False(t.ShowPatientAddress);
            Assert.False(t.ShowPatientPhysician);
            Assert.False(t.ShowPatientWard);
            Assert.False(t.ShowPatientPaymentMethod);
            Assert.False(t.ShowReferenceRanges);
            Assert.False(t.ShowConclusion);
            Assert.False(t.ShowFooter);
        }
    }
}
