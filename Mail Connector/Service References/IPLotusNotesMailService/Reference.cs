﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mail_Connector.IPLotusNotesMailService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Email", Namespace="http://schemas.datacontract.org/2004/07/Company.ServiceModel.IPMailServiceApp")]
    [System.SerializableAttribute()]
    public partial class Email : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Mail_Connector.IPLotusNotesMailService.Attachment[] AttachmentsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BodyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FromField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SubjectField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ToField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Mail_Connector.IPLotusNotesMailService.Attachment[] Attachments {
            get {
                return this.AttachmentsField;
            }
            set {
                if ((object.ReferenceEquals(this.AttachmentsField, value) != true)) {
                    this.AttachmentsField = value;
                    this.RaisePropertyChanged("Attachments");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Body {
            get {
                return this.BodyField;
            }
            set {
                if ((object.ReferenceEquals(this.BodyField, value) != true)) {
                    this.BodyField = value;
                    this.RaisePropertyChanged("Body");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> Date {
            get {
                return this.DateField;
            }
            set {
                if ((this.DateField.Equals(value) != true)) {
                    this.DateField = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string From {
            get {
                return this.FromField;
            }
            set {
                if ((object.ReferenceEquals(this.FromField, value) != true)) {
                    this.FromField = value;
                    this.RaisePropertyChanged("From");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Subject {
            get {
                return this.SubjectField;
            }
            set {
                if ((object.ReferenceEquals(this.SubjectField, value) != true)) {
                    this.SubjectField = value;
                    this.RaisePropertyChanged("Subject");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string To {
            get {
                return this.ToField;
            }
            set {
                if ((object.ReferenceEquals(this.ToField, value) != true)) {
                    this.ToField = value;
                    this.RaisePropertyChanged("To");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Attachment", Namespace="http://schemas.datacontract.org/2004/07/Company.ServiceModel.IPMailServiceApp")]
    [System.SerializableAttribute()]
    public partial class Attachment : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] ContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FileNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FilePathField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StatusField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FileName {
            get {
                return this.FileNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FileNameField, value) != true)) {
                    this.FileNameField = value;
                    this.RaisePropertyChanged("FileName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FilePath {
            get {
                return this.FilePathField;
            }
            set {
                if ((object.ReferenceEquals(this.FilePathField, value) != true)) {
                    this.FilePathField = value;
                    this.RaisePropertyChanged("FilePath");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Status {
            get {
                return this.StatusField;
            }
            set {
                if ((object.ReferenceEquals(this.StatusField, value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Company.ServiceModel.IPMailServiceApp", ConfigurationName="IPLotusNotesMailService.IIPLotusNotesMail")]
    public interface IIPLotusNotesMail {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/GetEmailAttachments", ReplyAction="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/GetEmailAttachmentsRe" +
            "sponse")]
        Mail_Connector.IPLotusNotesMailService.Attachment[] GetEmailAttachments(Mail_Connector.IPLotusNotesMailService.Email email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/GetMail", ReplyAction="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/GetMailResponse")]
        Mail_Connector.IPLotusNotesMailService.Email[] GetMail(bool getAllMail);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/GetData", ReplyAction="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/GetDataResponse")]
        Mail_Connector.IPLotusNotesMailService.Email[] GetData();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/SetMailAsRead", ReplyAction="http://Company.ServiceModel.IPMailServiceApp/IIPLotusNotesMail/SetMailAsReadResponse" +
            "")]
        bool SetMailAsRead(System.DateTime dateTime);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IIPLotusNotesMailChannel : Mail_Connector.IPLotusNotesMailService.IIPLotusNotesMail, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IPLotusNotesMailClient : System.ServiceModel.ClientBase<Mail_Connector.IPLotusNotesMailService.IIPLotusNotesMail>, Mail_Connector.IPLotusNotesMailService.IIPLotusNotesMail {
        
        public IPLotusNotesMailClient() {
        }
        
        public IPLotusNotesMailClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IPLotusNotesMailClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IPLotusNotesMailClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IPLotusNotesMailClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Mail_Connector.IPLotusNotesMailService.Attachment[] GetEmailAttachments(Mail_Connector.IPLotusNotesMailService.Email email) {
            return base.Channel.GetEmailAttachments(email);
        }
        
        public Mail_Connector.IPLotusNotesMailService.Email[] GetMail(bool getAllMail) {
            return base.Channel.GetMail(getAllMail);
        }
        
        public Mail_Connector.IPLotusNotesMailService.Email[] GetData() {
            return base.Channel.GetData();
        }
        
        public bool SetMailAsRead(System.DateTime dateTime) {
            return base.Channel.SetMailAsRead(dateTime);
        }
    }
}