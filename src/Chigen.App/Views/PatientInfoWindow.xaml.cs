using System.Windows;
using Chigen.App.ViewModels;
using Chigen.Core.Services;

namespace Chigen.App.Views
{
    public partial class PatientInfoWindow : Window
    {
        public PatientInfo Result { get; private set; } = new();

        public PatientInfoWindow()
        {
            InitializeComponent();
            var template = TemplateService.LoadTemplate();
            patientNamePanel.Visibility = template.ShowPatientName ? Visibility.Visible : Visibility.Collapsed;
            patientIdPanel.Visibility = template.ShowPatientId ? Visibility.Visible : Visibility.Collapsed;
            patientDobPanel.Visibility = template.ShowPatientDob ? Visibility.Visible : Visibility.Collapsed;
            patientSexPanel.Visibility = template.ShowPatientSex ? Visibility.Visible : Visibility.Collapsed;
            patientDiagnosisPanel.Visibility = template.ShowPatientDiagnosis ? Visibility.Visible : Visibility.Collapsed;
            patientAddressPanel.Visibility = template.ShowPatientAddress ? Visibility.Visible : Visibility.Collapsed;
            patientPhysicianPanel.Visibility = template.ShowPatientPhysician ? Visibility.Visible : Visibility.Collapsed;
            patientWardPanel.Visibility = template.ShowPatientWard ? Visibility.Visible : Visibility.Collapsed;
            patientPaymentPanel.Visibility = template.ShowPatientPaymentMethod ? Visibility.Visible : Visibility.Collapsed;
            txtName.Focus();
        }

        public PatientInfoWindow(PatientInfo existing) : this()
        {
            txtName.Text = existing.Name;
            txtId.Text = existing.Id;
            txtDob.Text = existing.DateOfBirth;
            txtSex.Text = existing.Sex;
            txtDiagnosis.Text = existing.Diagnosis;
            txtAddress.Text = existing.Address;
            txtPhysician.Text = existing.Physician;
            txtWard.Text = existing.Ward;
            txtPaymentMethod.Text = existing.PaymentMethod;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Result = new PatientInfo
            {
                Name = txtName.Text.Trim(),
                Id = txtId.Text.Trim(),
                DateOfBirth = txtDob.Text.Trim(),
                Sex = txtSex.Text.Trim(),
                Diagnosis = txtDiagnosis.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Physician = txtPhysician.Text.Trim(),
                Ward = txtWard.Text.Trim(),
                PaymentMethod = txtPaymentMethod.Text.Trim()
            };
            DialogResult = true;
            Close();
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

    public class PatientInfo
    {
        public string Name { get; set; } = "";
        public string Id { get; set; } = "";
        public string DateOfBirth { get; set; } = "";
        public string Sex { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string Address { get; set; } = "";
        public string Physician { get; set; } = "";
        public string Ward { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
