using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lims.Common.Dtos
{
    public class ParallelTestingDto : BaseDto
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string? parentId;

        public string? ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        private string? sampleWeight;

        public string? SampleWeight
        {
            get { return sampleWeight; }
            set { sampleWeight = value; RaisePropertiyChanged(nameof(SampleWeight)); }
        }

        private string? testResult;

        public string? TestResult
        {
            get { return testResult; }
            set { testResult = value; RaisePropertiyChanged(nameof(TestResult)); }
        }




        private int parallelIndex;

        public int ParallelIndex
        {
            get { return parallelIndex; }
            set { parallelIndex = value; RaisePropertiyChanged(nameof(ParallelIndex)); }
        }

        private string? attachmentPath;

        public string? AttachmentPath
        {
            get { return attachmentPath; }
            set { attachmentPath = value; RaisePropertiyChanged(nameof(AttachmentPath)); }
        }









    }
}
