using Chigen.Core.Models;

namespace Chigen.Core.Services
{
    public static class CellTypeProvider
    {
        public static List<CellType> GetDefaultPeripheralBloodTypes()
        {
            return
            [
                new() { Id = "neutrophil", Name = "Neutrophil (Seg)", Key = "1", ReferenceRange = "40-80", ReferenceLow = 40, ReferenceHigh = 80, Mode = CounterMode.PeripheralBlood },
                new() { Id = "band", Name = "Band Neutrophil", Key = "2", ReferenceRange = "0-10", ReferenceLow = 0, ReferenceHigh = 10, Mode = CounterMode.PeripheralBlood },
                new() { Id = "lymphocyte", Name = "Lymphocyte", Key = "3", ReferenceRange = "20-40", ReferenceLow = 20, ReferenceHigh = 40, Mode = CounterMode.PeripheralBlood },
                new() { Id = "monocyte", Name = "Monocyte", Key = "4", ReferenceRange = "2-10", ReferenceLow = 2, ReferenceHigh = 10, Mode = CounterMode.PeripheralBlood },
                new() { Id = "eosinophil", Name = "Eosinophil", Key = "5", ReferenceRange = "1-6", ReferenceLow = 1, ReferenceHigh = 6, Mode = CounterMode.PeripheralBlood },
                new() { Id = "basophil", Name = "Basophil", Key = "6", ReferenceRange = "0-2", ReferenceLow = 0, ReferenceHigh = 2, Mode = CounterMode.PeripheralBlood },
                new() { Id = "metamyelocyte", Name = "Metamyelocyte", Key = "7", ReferenceRange = "0-5", ReferenceLow = 0, ReferenceHigh = 5, Mode = CounterMode.PeripheralBlood },
                new() { Id = "myelocyte", Name = "Myelocyte", Key = "8", ReferenceRange = "0-3", ReferenceLow = 0, ReferenceHigh = 3, Mode = CounterMode.PeripheralBlood },
                new() { Id = "promyelocyte", Name = "Promyelocyte", Key = "9", ReferenceRange = "0-2", ReferenceLow = 0, ReferenceHigh = 2, Mode = CounterMode.PeripheralBlood },
                new() { Id = "monoblast", Name = "Monoblast", Key = "Q", ReferenceRange = "0-1", ReferenceLow = 0, ReferenceHigh = 1, Mode = CounterMode.PeripheralBlood },
                new() { Id = "lymphoblast", Name = "Lymphoblast", Key = "W", ReferenceRange = "0-1", ReferenceLow = 0, ReferenceHigh = 1, Mode = CounterMode.PeripheralBlood },
                new() { Id = "other", Name = "Other", Key = "O", ReferenceRange = "0-0", ReferenceLow = 0, ReferenceHigh = 0, Mode = CounterMode.PeripheralBlood },
            ];
        }

        public static List<CellType> GetDefaultBoneMarrowTypes()
        {
            return
            [
                new() { Id = "neutrophil", Name = "Neutrophil (Seg)", Key = "1", ReferenceRange = "40-80", ReferenceLow = 40, ReferenceHigh = 80, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "band", Name = "Band Neutrophil", Key = "2", ReferenceRange = "0-10", ReferenceLow = 0, ReferenceHigh = 10, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "lymphocyte", Name = "Lymphocyte", Key = "3", ReferenceRange = "20-40", ReferenceLow = 20, ReferenceHigh = 40, Mode = CounterMode.BoneMarrow, Group = "Lymphoid" },
                new() { Id = "monocyte", Name = "Monocyte", Key = "4", ReferenceRange = "2-10", ReferenceLow = 2, ReferenceHigh = 10, Mode = CounterMode.BoneMarrow, Group = "Monocytic" },
                new() { Id = "eosinophil", Name = "Eosinophil", Key = "5", ReferenceRange = "1-6", ReferenceLow = 1, ReferenceHigh = 6, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "basophil", Name = "Basophil", Key = "6", ReferenceRange = "0-2", ReferenceLow = 0, ReferenceHigh = 2, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "metamyelocyte", Name = "Metamyelocyte", Key = "7", ReferenceRange = "0-5", ReferenceLow = 0, ReferenceHigh = 5, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "myelocyte", Name = "Myelocyte", Key = "8", ReferenceRange = "0-3", ReferenceLow = 0, ReferenceHigh = 3, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "promyelocyte", Name = "Promyelocyte", Key = "9", ReferenceRange = "0-2", ReferenceLow = 0, ReferenceHigh = 2, Mode = CounterMode.BoneMarrow, Group = "Myeloid" },
                new() { Id = "nrbC", Name = "NRBC", Key = "A", ReferenceRange = "0-0", ReferenceLow = 0, ReferenceHigh = 0, Mode = CounterMode.BoneMarrow, Group = "Erythroid" },
                new() { Id = "megakaryocyte", Name = "Megakaryocyte", Key = "B", ReferenceRange = "0-2", ReferenceLow = 0, ReferenceHigh = 2, Mode = CounterMode.BoneMarrow, Group = "Megakaryocytic" },
                new() { Id = "plasmaCell", Name = "Plasma Cell", Key = "C", ReferenceRange = "0-2", ReferenceLow = 0, ReferenceHigh = 2, Mode = CounterMode.BoneMarrow, Group = "Lymphoid" },
                new() { Id = "monocytePrecursor", Name = "Monocyte Precursor", Key = "D", ReferenceRange = "0-1", ReferenceLow = 0, ReferenceHigh = 1, Mode = CounterMode.BoneMarrow, Group = "Monocytic" },
                new() { Id = "erythroblast", Name = "Erythroblast", Key = "E", ReferenceRange = "0-5", ReferenceLow = 0, ReferenceHigh = 5, Mode = CounterMode.BoneMarrow, Group = "Erythroid" },
                new() { Id = "monoblast", Name = "Monoblast", Key = "Q", ReferenceRange = "0-1", ReferenceLow = 0, ReferenceHigh = 1, Mode = CounterMode.BoneMarrow, Group = "Blast" },
                new() { Id = "lymphoblast", Name = "Lymphoblast", Key = "W", ReferenceRange = "0-1", ReferenceLow = 0, ReferenceHigh = 1, Mode = CounterMode.BoneMarrow, Group = "Blast" },
                new() { Id = "other", Name = "Other", Key = "O", ReferenceRange = "0-0", ReferenceLow = 0, ReferenceHigh = 0, Mode = CounterMode.BoneMarrow, Group = "Other" },
            ];
        }
    }
}
