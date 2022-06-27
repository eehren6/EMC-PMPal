using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Linq;
using PMPal.Models;
using SelectPdf;

namespace PMPal.Backend
{
    public class DentalReferralNote
    {
        public Dictionary<string, PatientReferral> patientReferrals { get; set; }
        Person RefPatient { get; set; }

        string EncID { get; set; }

        Provider ReferringProvider { get; set; }

        string ReferralProvider { get; set; }

        string TeethCondition { get; set; }

        string Specialty { get; set; }

        string Reason { get; set; }

        string OfficeInstruction { get; set; }

        string Procedure { get; set; }

        string Status { get; set; }

        DateTime NoteDate { get; set; }

        public DentalReferralNote(string personID, string encID)
        {
            RefPatient = Patient.GetPatientInfo(personID);
            EncID = encID;

            GetNoteDataFromDB();

            //xml = "<Body><Paragraph><Replacement Id=\"C48\" tableColumn=\"edrnt_referrals.ToothNo_Condition\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Tooth No. or Condition:</Run></Le><Op><Run Foreground=\"#FF0000FF\">2</Run></Op><Tr/></Replacement></Paragraph><Paragraph><Replacement Id=\"9B5\" tableColumn=\"edrnt_referrals.Referred\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Patient Referred: </Run></Le><Op><Run Foreground=\"#FF0000FF\">No</Run></Op><Tr/></Replacement></Paragraph><Paragraph><Replacement Id=\"A1B\" tableColumn=\"edrnt_referrals.specialty\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Specialty: </Run></Le><Op><Run Foreground=\"#FF0000FF\">Oral Surgery;</Run></Op><Tr><Run Foreground=\"#FF0000FF\">. </Run></Tr></Replacement></Paragraph><Paragraph><Replacement Id=\"DA9\" tableColumn=\"edrnt_referrals.referred_to\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Specialist: </Run></Le><Op><Run Foreground=\"#FF0000FF\">Dr. Rosenberg;</Run></Op><Tr><Run Foreground=\"#FF0000FF\">. </Run></Tr></Replacement></Paragraph><Paragraph><Replacement Id=\"0B0\" tableColumn=\"edrnt_referrals.reason\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Reason: </Run></Le><Op><Run Foreground=\"#FF0000FF\">Endo</Run></Op><Tr><Run Foreground=\"#FF0000FF\">. </Run></Tr></Replacement></Paragraph><Paragraph><Replacement Id=\"B21\" tableColumn=\"edrnt_referrals.office_instruction\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Office Instruction: </Run></Le><Op><Run Foreground=\"#FF0000FF\">Urgent - follow up in about one week</Run></Op><Tr><Run Foreground=\"#FF0000FF\">. </Run></Tr></Replacement></Paragraph><Paragraph><Replacement Id=\"4FF\" tableColumn=\"edrnt_referrals.status\"><Le><Run FontWeight=\"Bold\" Foreground=\"#FF0000FF\">Status: </Run></Le><Op><Run Foreground=\"#FF0000FF\">Ordered</Run></Op><Tr/></Replacement></Paragraph></Body>";
            //RetrieveFromXML(xml);
        }
        
        private void GetNoteDataFromDB()
        {
            string sql = $@"select epc.patient_note_id, epc.note,epc.provider_id, um.user_id, pm.first_name, pm.last_name, pm.salutory_name, pm.degree, ml.mstr_list_item_desc type, note_date
                from edr_patient_notes epc
                join provider_mstr pm on pm.provider_id = epc.provider_id
                join user_mstr um on pm.provider_id = um.provider_id                
                join mstr_lists ml on ml.mstr_list_item_id = pm.provider_subgrouping1_id
                 where epc.description='Referral'
                        and person_id = '{RefPatient.person_id}' and encounter_id = '{EncID}'  and epc.delete_ind='N'";



            var ds = Util.Get(sql, System.Data.CommandType.Text);
            if (ds.Tables != null && ds.Tables.Count == 1)
            {
                patientReferrals = new Dictionary<string, PatientReferral>();

                foreach (DataRow record in ds.Tables[0].Rows)
                {
                    PatientReferral patientReferral = new PatientReferral();
                    patientReferral.ReferralID = record["patient_note_id"].ToString();
                    patientReferral.PersonID = RefPatient.person_id;
                    patientReferral.RefPatient = RefPatient;
                    patientReferral.EncID = EncID;
                    patientReferral.Note = record["note"].ToString();
                    patientReferral.NoteDate = Convert.ToDateTime(record["note_date"]);
                    RetrieveFromXML(patientReferral);

                    ReferringProvider = new Provider();
                    ReferringProvider.person_id = record["provider_id"].ToString();
                    ReferringProvider.First_name = record["First_name"].ToString();
                    ReferringProvider.Last_name = record["last_name"].ToString();
                    ReferringProvider.UserID = record["user_id"].ToString();
                    ReferringProvider.Salutary = record["salutory_name"].ToString();
                    ReferringProvider.Degree = record["Degree"].ToString();
                    ReferringProvider.Subgrouping = record["type"].ToString();
                    ReferringProvider.Description = "";
                    patientReferral.ReferringProvider = ReferringProvider;


                    patientReferrals.Add(patientReferral.ReferralID, patientReferral);
                }
            }
        }
        public void GenerateReferralLetter(string noteID)
        {
            try
            {
                var referral = patientReferrals[noteID];
                string html = GenerateHTMLDocument(referral);
                GeneratePDF(html);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RetrieveFromXML(PatientReferral referral)
        {
            try
            {
                string xml = "<root>" + referral.Note + "</root>";
                XElement referralDetails = XElement.Parse(xml);
                foreach (XElement element in referralDetails.Elements())
                {
                    string sentence = element.Value.ToString();
                    string value = sentence.Length > sentence.IndexOf(':') ? sentence[(sentence.IndexOf(':') + 1)..].Trim().Trim(' ', ';', '.', '!') : "";

                    if (sentence.Contains("Tooth"))
                    {
                        referral.TeethCondition = value;
                    }
                    if (sentence.Contains("Specialist"))
                    {
                        referral.ReferralProvider = value;
                    }
                    if (sentence.Contains("Reason"))
                    {
                        referral.Reason = value;
                    }
                    if (sentence.Contains("Specialty"))
                    {
                        referral.Specialty = value;
                    }
                    if (sentence.Contains("Office"))
                    {
                        referral.OfficeInstruction = value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Specialty = referralDetails.Elements("").
        }


        private string GenerateHTMLDocument(PatientReferral referral)
        {
            try
            {

                string html = @"<html><head><meta content=""text/html; charset=UTF-8"" http-equiv=""content-type"">
<style type=""text/css""

>@import url('https://themes.googleusercontent.com/fonts/css?kit=wAPX1HepqA24RkYW1AuHYA');ol{margin:0;padding:0}table td,table th{padding:0}
.c1{color:#000000;text-decoration:none;vertical-align:baseline;font-size:14pt;font-family:""Calibri"";font-style:normal}
.c0{padding-top:0pt;padding-bottom:0pt;line-height:1.0;orphans:2;widows:2;text-align:left;height:11pt}
.c2{margin-left:432pt;padding-top:0pt;padding-bottom:0pt;line-height:1.0;orphans:2;widows:2;text-align:left}
.c4{padding-top:0pt;padding-bottom:0pt;line-height:1.5;orphans:2;widows:2;text-align:left;height:11pt}
.c9{padding-top:0pt;padding-bottom:0pt;line-height:1.5;orphans:2;widows:2;text-align:left}
.c3{padding-top:0pt;padding-bottom:0pt;line-height:1.5;orphans:2;widows:2;text-align:left}
.c10{padding-top:0pt;padding-bottom:0pt;line-height:1.0;text-align:left}
.c11{-webkit-text-decoration-skip:none;color:#0000ff;text-decoration:underline;text-decoration-skip-ink:none}
.c13{vertical-align:baseline;font-size:11pt;font-style:normal}
.c14{background-color:#ffffff;max-width:468pt;padding:30pt 32pt 121.5pt 72pt}
.c6{font-weight:400;font-family:""Calibri""}
.c5{color:inherit;text-decoration:inherit}
.c7{border:1px solid black;margin:5px}
.c12{color:#000000;text-decoration:none}
.title{padding-top:24pt;color:#000000;font-weight:700;font-size:36pt;padding-bottom:6pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}
.subtitle{padding-top:18pt;color:#666666;font-size:24pt;padding-bottom:4pt;font-family:""Georgia"";line-height:1.0;page-break-after:avoid;font-style:italic;orphans:2;widows:2;text-align:left}
li{color:#000000;font-size:11pt;font-family:""Times New Roman""}
p{margin:0;color:#000000;font-size:11pt;font-family:""Times New Roman""}
h1{padding-top:24pt;color:#000000;font-weight:700;font-size:24pt;padding-bottom:6pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}
h2{padding-top:18pt;color:#000000;font-weight:700;font-size:18pt;padding-bottom:4pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}
h3{padding-top:14pt;color:#000000;font-weight:700;font-size:14pt;padding-bottom:4pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}
h4{padding-top:12pt;color:#000000;font-weight:700;font-size:12pt;padding-bottom:2pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}
h5{padding-top:11pt;color:#000000;font-weight:700;font-size:11pt;padding-bottom:2pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}
h6{padding-top:10pt;color:#000000;font-weight:700;font-size:10pt;padding-bottom:2pt;font-family:""Times New Roman"";line-height:1.0;page-break-after:avoid;orphans:2;widows:2;text-align:left}

</style></head>
<body class=""c14"">
<div>
    <a id=""id.gjdgxs""></a>
    <p class=""c3"">
        <span style=""overflow: hidden; display: inline-block; margin: 0.00px 0.00px; border: 0.00px solid #000000; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px); width: 173.73px; height: 79.73px;"">
            <img alt="""" src=""#images/image5.png"" style=""width: 173.73px; height: 79.73px; margin-left: -0.00px; margin-top: -0.00px; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px);"" title="""">
        </span>
    </p>
</div>

<span class=""c1"">
    <span class=""c2"">#Date</span>
    <span style=""overflow: hidden; display: inline-block; margin: 0.00px 0.00px; border: 0.00px solid #000000; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px); width: 746.00px; height: 3.00px;"">
        <img alt="""" src=""#images/image2.png"" style=""width: 773.00px; height: 3.00px; margin-left: 0.00px; margin-top: 0.00px; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px);"" title="""">
    </span>

<p class=""c4""><span class=""c1""></span></p>

<p class=""c3""><span class=""c1"">Patient name: #FName #LName 
<span style=""float: right"" margin: 0>Specialty: #Spec</span></span></p>

<p class=""c3""><span class=""c1"">DOB: #DOB
<span style=""float: right"">Specialist: #SpcProv</span></span></p>


<p class=""c3""><span class=""c1"">Phone Number: #Phone 
<span style=""float: right"">Specialist Address</span></span></p>

<p class=""c3""><span class=""c1"">Referring Doctor: #Prov 
<span style=""float: right"">Specialist Phone Number</span></span></p>
<p class=""c4""><span class=""c1""></span></p>

<span style=""overflow: hidden; display: inline-block; margin: 0.00px 0.00px; border: 0.00px solid #000000; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px); width: 746.00px; height: 3.00px;"">
<img alt="""" src=""#images/image2.png"" style=""width: 746.00px; height: 3.00px; margin-left: 0.00px; margin-top: 0.00px; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px);"" title=""""></span></p>
<p class=""c0""><span class=""c1""></span></p>
<span sty
<p class=""c9""><span class=""c1"">&nbsp;Dear #SpcProv,</span></p>
<p class=""c4""><span class=""c1""></span></p>
<p class=""c9""><span class=""c1"">The above patient was seen at our office on #NoteDate. Please evaluate the above patient for #Proc #Teeth and treat as indicated.</span></p>
<p class=""c4""><span class=""c1""></span></p>
<p class=""c9""><span class=""c1"">#Instruct</span></p>
<p class=""c4""><span class=""c1""></span></p>
<p class=""c0""><span class=""c1""></span></p>
<p class=""c0""><span class=""c1""></span></p>
<p class=""c9""><span class=""c1"">Please either email or fax the report and x-rays to the Dental Records Department at Ezra Medical Center. <br/>
Email: <a class=""c5"" href=""mailto:dentalrecords@ezramedical.org"">dentalrecords@ezramedical.org</a><br/>
Phone Number: 718-686-7600 ext. 2032<br/>
Fax: 1-888-385-7941</span>
<p class=""c4""><span class=""c1""></span></p>
<p class=""c0""><span class=""c1""></span></p>
<p class=""c3""><span class=""c1"">Thank you,</span></p>
<p class=""c0""><span class=""c1""></span></p>
<p class=""c3""><span class=""c1"">#Prov</span></p>
<p class=""c3""><span class=""c1"">#Type</span></p>
<p class=""c3""><span class=""c1"">Ezra Medical Center</span></p>
<p class=""c3""><span class=""c1"">
<img alt="""" src=""#sigImage"" style=""width: 200.00px; height: 40px; margin-left: -0.00px; margin-top: -0.00px; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px);"" title=""""></span>
</span></p>
<br/>
<br/>
<p class=""c3""><span style=""overflow: hidden; display: inline-block; margin: 0.00px 0.00px; border: 0.00px solid #000000; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px); width: 746.00px; height: 3.00px;"">
<img alt="""" src=""#images/image2.png"" style=""width: 746.00px; height: 3.00px; margin-left: 0.00px; margin-top: 0.00px; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px);"" title=""""></span></p>

<p class=""c3"">
<span style=""overflow: hidden; display: inline-block; margin: 0.00px 0.00px; border: 0.00px solid #000000; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px); width: 135.00px; height: 148.40px;"">

<span id=""Frame3"" dir=""ltr"" style=""float: left; width: 1.09in; border: none; padding: 0in; background: #ffffff""><p align=""justify"" style=""margin-bottom: 0in; line-height: 100%"">
		<font color=""#002152""><font face=""Arial, serif""><font size=""1"" style=""font-size: 7pt"">EzraMedical.org</font></font></font></p>
		<p style=""margin-bottom: 0in; line-height: 100%""><br/>

		</p>
	</span>
</span
</p>
    <span id=""Frame1"" dir=""ltr"" style=""float: left; width: 1.72in; height: 0.68in; border: none; padding: 0in; background: #ffffff"">
            <p style=""margin-right: 0.05in; margin-bottom: 0in; line-height: 115%"">
		    <font color=""#002152"" face=""Arial, serif"" size=""1"" style=""font-size: 8pt"">1278 60th Street<br/>Brooklyn, NY 11219<br/>
		    Phone(718) 741-7100</font></p>
    </span>

	<span id=""Frame2"" dir=""ltr"" style=""float: left; width: 1.4in; border: none; padding: 0in; background: #ffffff"">
        <p style=""margin-bottom: 0in; line-height: 115%"">
		<font color=""#002152"" face=""Arial, serif"" size=""1"" style=""font-size: 8pt"">1312
		38th Street<br/>Brooklyn,
		NY 11218<br/>Phone
		(718) 686-7600</font></p>
	</span>
    <span style=""overflow: hidden; display: inline-block; margin: 0.00px 0.00px; border: 0.00px solid #000000; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px); width: 887.00px; height: 71.17px;"">

<img alt="""" src=""#images/image3.jpg"" style=""width: 887.00px; height: 40px; margin-left: -0.00px; margin-top: -0.00px; transform: rotate(0.00rad) translateZ(0px); -webkit-transform: rotate(0.00rad) translateZ(0px);"" title=""""></span>
</p>
</body></html>";


                html = html.Replace("#FName", RefPatient.First_name);
                html = html.Replace("#LName", RefPatient.Last_name);
                html = html.Replace("#PNBR", RefPatient.person_nbr);
                html = html.Replace("#DOB", RefPatient.DOB_display);
                html = html.Replace("#Phone", RefPatient.Masked_Home_Phone != "" ? RefPatient.Masked_Home_Phone : RefPatient.Cell_Phone_Display);
                html = html.Replace("#Email", RefPatient.email_address);
                html = html.Replace("#Address", RefPatient.PrimaryAddress.StreetAddress1);
                html = html.Replace("#NoteDate", referral.NoteDate.ToShortDateString());
                html = html.Replace("#Date", DateTime.Now.ToShortDateString());
                html = html.Replace("#Prov", $"{ referral.ReferringProvider.First_name} {referral.ReferringProvider.Last_name} {referral.ReferringProvider.Degree}");
                html = html.Replace("#Type", referral.ReferringProvider.Subgrouping);
                html = html.Replace("#Spec", referral.Specialty);
                html = html.Replace("#SpcProv", referral.ReferralProvider.Contains("Dr") ? referral.ReferralProvider : "Specialist");
                html = html.Replace("#Teeth", referral.TeethCondition.Length > 0 && (referral.TeethCondition.Length <= 2 || referral.TeethCondition == "#1,#16,#17,#32")
                                                                                        ? "Tooth: " + referral.TeethCondition : referral.TeethCondition);
                html = html.Replace("#Instruct", ""); //removed: referral.OfficeInstruction.Length > 0 ? "Additional Instructions: " + referral.OfficeInstruction : "");
                html = html.Replace("#Proc", referral.Procedure);

                var imageloc = Environment.CurrentDirectory + "/images";
                html = html.Replace("#images", imageloc);
                var prov = referral.ReferringProvider;
                html = html.Replace("#sigImage", ProviderSignatureLocation(prov.First_name, prov.Last_name, prov.UserID));

                return html;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GeneratePDF(string html)
        {
            try
            {
                HtmlToPdf converter = new HtmlToPdf();
                var doc = converter.ConvertHtmlString(html);
                //string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //string filename = folderPath + "\\refdoc" + DateTime.Now.ToString("ddMM-hhmm") + ".pdf";

                string folderPath = "\\\\SERVERFILE-A\\EDRDocs";
                string newDataID = Guid.NewGuid().ToString();
                string filename = folderPath + "\\" + newDataID.Replace("-","") + ".pdf";
                doc.Save(filename);
                doc.Close();

                SavePDF(newDataID, RefPatient.person_id, folderPath);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void SavePDF(string newDataID, string personID, string name)
        {
            try
            {
                string newID = Guid.NewGuid().ToString();
                string user_id = Util.GetActiveUser();
                string sql = $@"
                INSERT INTO [dbo].[edr_patient_documents]
                   ([enterprise_id]
                   ,[practice_id]
                   ,[patient_document_id]
                   ,[person_id]
                   ,[document_category_type_id]
                   ,[description]
                   ,[delete_ind]
                   ,[provider_id]
                   ,[create_timestamp]
                   ,[created_by]
                   ,[modify_timestamp]
                   ,[modified_by])
                values
                    ('00001','0001',
                    '{newID}',
                    '{personID}',
                    'DEE0E163-5998-42DE-A057-4F186593CC34', --referral report
                    'Referral',
                    'N',
                    '{ReferringProvider.person_id}',
                    '{DateTime.Now:yyyy-MM-dd hh:mm}','{user_id}',
                    '{DateTime.Now:yyyy-MM-dd hh:mm}','{user_id}');

                INSERT INTO [dbo].[edr_patient_document_data]
                   ([patient_document_data_id]
                   ,[patient_document_id]
                   ,[data]
                   ,[data_length]
                   ,[file_extension]
                   ,[data_type]
                   ,[page_number]
                   ,[create_timestamp]
                   ,[created_by]
                   ,[modify_timestamp]
                   ,[modified_by]
                   ,[name])
                values
                    ('{newDataID}',
                    '{newID}',
                    Convert(varbinary(max),0x),
                    0,
                    'pdf',
                    2,
                    1,
                    '{DateTime.Now:yyyy-MM-dd hh:mm}','{user_id}',
                    '{DateTime.Now:yyyy-MM-dd hh:mm}','{user_id}',
                    '{name}');
                ";

                Util.Update(sql, CommandType.Text, null, null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string ProviderSignatureLocation(string providerFirstName, string providerLastName, string providerUserID)
        {
            try
            {
                string locationFolder = Util.GetSignatureFolder();
                var localImageFolder = Environment.CurrentDirectory + "/images/";
                string fileName = $"{locationFolder}{providerFirstName}_{providerLastName}_{providerUserID}.jpg";

                /*TODO: create more permanent method*/
                string localFileName = $"{localImageFolder}{providerFirstName}_{providerLastName}_{providerUserID}.jpg";
                if(!File.Exists(localFileName))
                    File.Copy(fileName, localFileName);
                
                return localFileName;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
