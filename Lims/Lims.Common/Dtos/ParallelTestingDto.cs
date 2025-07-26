using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lims.Common.Dtos
{
    public class ParallelTestingDto : BaseDto
    {
        public ParallelTestingDto()
        {
            
        }
        public ParallelTestingDto(string ItemId)
        {
            this.ItemId = ItemId;
        }
        private string? itemId;

        public string? ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
            }
        }

        private string? firstSampleWeight;

        public string? FirstSampleWeight
        {
            get
            {
                return firstSampleWeight;
            }
            set
            {
                firstSampleWeight = value;
                RaisePropertiyChanged(nameof(FirstSampleWeight));
            }
        }
        private string? secondSampleWeight;

        public string? SecondSampleWeight
        {
            get
            {
                return secondSampleWeight;
            }
            set
            {
                secondSampleWeight = value;
                RaisePropertiyChanged(nameof(SecondSampleWeight));
            }
        }
        private string? firstTestResult;

        public string? FirstTestResult
        {
            get
            {
                return firstTestResult;
            }
            set
            {
                firstTestResult = value;
                RaisePropertiyChanged(nameof(FirstTestResult));
            }
        }
        private string? secondTestResult;

        public string? SecondTestResult
        {
            get
            {
                return secondTestResult;
            }
            set
            {
                secondTestResult = value;
                RaisePropertiyChanged(nameof(SecondTestResult));
            }
        }                

        private string? firstAttachmentPath;

        public string? FirstAttachmentPath
        {
            get
            {
                return firstAttachmentPath;
            }
            set
            {
                firstAttachmentPath = value;
                RaisePropertiyChanged(nameof(FirstAttachmentPath));
            }
        }
        private string? secondAttachmentPath;

        public string? SecondAttachmentPath
        {
            get
            {
                return secondAttachmentPath;
            }
            set
            {
                secondAttachmentPath = value;
                RaisePropertiyChanged(nameof(SecondAttachmentPath));
            }
        }








    }
}
