using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Company.ServiceModel.LotusNotesMailService
{
    [ServiceContract(Namespace = "http://Company.ServiceModel.LotusNotesMailService.ILotusNotesMail")]
    public interface ILotusNotesMail
    {
        [OperationContract]
        List<Attachment> GetEmailAttachments(Email email);

        [OperationContract]
        List<Email> GetMail(bool getAllMail);

        [OperationContract]
        List<Email> GetData();

        [OperationContract]
        void SetMailAsRead(DateTime dateTime);
    }

    [DataContract]
    public class Email
    {
        [DataMember]
        public DateTime? Date;

        [DataMember]
        public String From;

        [DataMember]
        public String Subject;

        [DataMember]
        public String Body;

        [DataMember]
        public String To;

        [DataMember]
        public List<Attachment> Attachments;
    }

    [DataContract]
    public class Attachment
    {
        public string FileNameOriginal;

        [DataMember]
        public string FileName;

        [DataMember]
        public string FilePath;

        [DataMember]
        public byte[] Content;

        [DataMember]
        public string Status;
    }
}