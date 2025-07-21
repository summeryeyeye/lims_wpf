using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lims.Common.Dtos
{
    public class ParalleltestingDto : BaseDto
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
        /// <summary>
        /// 取样量
        /// </summary>
        private string? weight;

        public string? Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                RaisePropertiyChanged(nameof(Weight));
            }
        }
        /// <summary>
        /// 序号
        /// </summary>
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertiyChanged(nameof(Index)); }
        }
        /// <summary>
        /// 检测结果
        /// </summary>
        private string? testResult;

        public string? TestResult
        {
            get { return testResult; }
            set { testResult = value; RaisePropertiyChanged(nameof(TestResult)); }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        private string? attachmentPath;

        public string? AttachmentPath
        {
            get { return attachmentPath; }
            set { attachmentPath = value; RaisePropertiyChanged(nameof(AttachmentPath)); }
        }

    }
}
