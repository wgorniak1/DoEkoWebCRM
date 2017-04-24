
namespace DoEko.ViewModels.SharedViewModels
{
    public class ModalConfirmationViewModel
    {
        public ModalConfirmationViewModel()
        {
            Id = "ModalConfirmation";
            BtnYes = "Tak";
            BtnNo = "Nie";
            BtnClose = true;

            Title = "Potwierdzenie";
            Question = "Potwierdź wybraną akcję";

            BtnYesClass = "modal-confirm";
            BtnNoClass = "modal-cancel";
            BtnCloseClass = "modal-cancel";
        }
        public string Id { get; set; }

        public string BtnYesClass { get; set; }
        public string BtnNoClass { get; set; }
        public string BtnCloseClass { get; set; }


        public string Title { get; set; }
        public string Question { get; set; }
        public string BtnYes { get; set; }
        public string BtnNo { get; set; }
        public bool BtnClose { get; set; }
    }
}
