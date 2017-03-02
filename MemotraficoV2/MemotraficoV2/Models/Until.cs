//using Excel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MemotraficoV2.Models
{
    public class Util
    {
        public class ImportStatus
        {
            public int errorCode { get; set; }
            public string message { get; set; }
            public bool result { get; set; }

            public ImportStatus()
            {
                errorCode = 0;
                result = false;
            }
        }


        #region Email

        public class Attachment
        {
            public string name { get; set; }
            public string data { get; set; }
        }

        /// <summary>
        /// Enviar en email utilizando un template html alcamenado en el servidor
        /// </summary>
        /// <param name="email">Destinatario</param>
        /// <param name="subject">Asunto</param>
        /// <param name="template">Nombre del archivo</param>
        /// <param name="parametros">Datos que tendrá el email</param>
        /// <param name="user">Remitente</param>
        /// <param name="attachment">Adjunto</param>
        public static void SendEmailWithTemplate(string email, string subject, string template, object parametros, string user = null, Attachment attachment = null)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/PlantillasEmail/");

            if (!string.IsNullOrEmpty(template))
            {
                try
                {
                    string html = File.ReadAllText(path + template);

                    foreach (var m in parametros.GetType().GetProperties())
                    {
                        html = html.Replace("{" + m.Name + "}", parametros.GetType().GetProperty(m.Name).GetValue(parametros, null).ToString());
                    }

                    if (string.IsNullOrEmpty(user))
                    {
                        user = ConfigurationManager.AppSettings["EmailDefault"];
                    }

                    SendEmail(user, email, html, subject, attachment);
                }
                catch (Exception)
                { }
            }


        }

        /// <summary>
        /// Enviar un email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email">Destinatario</param>
        /// <param name="message"></param>
        /// <param name="subject"></param>
        public static void SendEmail(string user, string email, string message, string subject, Attachment attachment = null)
        {
            NameValueCollection appConfig = ConfigurationManager.AppSettings;
            string userName = appConfig["EmailUser"],
                mask = appConfig["EmailMask"],
                password = appConfig["EmailPass"],
                host = appConfig["EmailHost"];
            int port = int.Parse(appConfig["EmailPort"]);

            MailMessage mail = new MailMessage();

            if (string.IsNullOrEmpty(user))
            {
                user = ConfigurationManager.AppSettings["EmailDefault"];
            }

            mail.To.Add(email);
            mail.From = new MailAddress(string.Format(mask, user), string.Format(mask, user), System.Text.Encoding.UTF8);
            mail.Subject = subject;
            mail.IsBodyHtml = true;

            if (attachment != null && attachment.data != null)
            {
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message, Encoding.UTF8, MediaTypeNames.Text.Html);
                String encodingPrefix = "base64,";
                int contentStartIndex = attachment.data.IndexOf(encodingPrefix) + encodingPrefix.Length;
                byte[] imageData = Convert.FromBase64String(attachment.data.Substring(contentStartIndex));

                LinkedResource img = new LinkedResource(new MemoryStream(imageData));
                img.ContentId = "image";
                img.TransferEncoding = TransferEncoding.Base64;
                img.ContentType.Name = !string.IsNullOrEmpty(attachment.name) ? attachment.name : string.Format("Archivo_{0}.png", DateTime.Now.ToString("MMddyyyyHmm"));
                img.ContentLink = new Uri("cid:" + img.ContentId);
                htmlView.LinkedResources.Add(img);

                mail.AlternateViews.Add(htmlView);
            }
            else
            {
                mail.Body = message;
            }

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Port = port;
            smtpClient.Host = host;
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion


        /// <summary>
        /// Crea un objecto dataset apartir de una hoja de calculo
        /// </summary>
        /// <param name="input">Datos del archivo</param>
        /// <returns>Devuelve un objeto de tipo dataset con los datos suministrados</returns>
        public static DataSet ToDataSet(StreamReader input)
        {
            DataSet data = new DataSet();
            data.Tables.Add();
            data.Tables[0].Columns.Add();
            int r = 0;
            while (!input.EndOfStream)
            {
                data.Tables[0].Rows.Add();
                string[] columns = input.ReadLine().Split(';');
                if (r == 0)
                {
                    for (int c = 0; c < columns.Length; c++)
                    {
                        data.Tables[0].Columns.Add();
                    }
                    //Se agrega una columna extra para el idCliente
                    data.Tables[0].Columns.Add();
                }
                for (int c = 0; c < columns.Length - 1; c++)
                {
                    data.Tables[0].Rows[r][c] = columns[c];
                }
                r++;
            }
            return data;
        }

        #region Importaciones

        public struct TipoArchivos
        {
            public const int CSV = 1;
            public const int EXCEL = 2;
            public const int EXCEL_2013 = 3;
        }

        /// <summary>
        /// Determina el tipo archivo que se sumistra
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>Devuelve unvalor numerico que representa el tipo de archivo</returns>
        public static int GetFileType(string fileName)
        {
            Regex reg = new Regex(@"\.xls");
            Regex reg2 = new Regex(@"\.xlsx");
            Regex reg3 = new Regex(@"\.csv");

            if (reg2.IsMatch(fileName))
                return TipoArchivos.EXCEL_2013;
            else if (reg.IsMatch(fileName))
                return TipoArchivos.EXCEL;
            else if (reg3.IsMatch(fileName))
                return TipoArchivos.CSV;
            else
                return 0;
        }

        /// <summary>
        /// Pasar la información de un archivo a un DataSet
        /// </summary>
        /// <param name="input">Datos del archivo</param>
        /// <returns>Devuelve un objeto de tipo DataSet con los datos suministrados</returns>
        //public static DataSet ToDataSet(HttpPostedFileBase input)
        //{

        //    if (input == null)
        //    {
        //        throw new Exception("No se ha especificado el archivo");
        //    }

        //    MemoryStream stream = new MemoryStream();
        //    input.InputStream.CopyTo(stream); // Se carga el archivo en memoria

        //    DataSet result = null;
        //    stream.Position = 0;
        //    int option = Util.GetFileType(input.FileName);

        //    // Leer el archivo dependiendo del tipo de archivo


        //    if (option == TipoArchivos.EXCEL_2013 || option == TipoArchivos.EXCEL)
        //    {
        //        // Lectura de los excel con ayuda de una dll
        //        IExcelDataReader reader;
        //        if (option == TipoArchivos.CSV)
        //            reader = ExcelReaderFactory.CreateBinaryReader(stream);
        //        else
        //            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        //        reader.IsFirstRowAsColumnNames = true;
        //        result = reader.AsDataSet();

        //        // Elimina filas y columnas vacias
        //        int table = 0;
        //        while (table < result.Tables.Count)
        //        {
        //            foreach (var column in result.Tables[table].Columns.Cast<DataColumn>().ToArray())
        //            {
        //                if (result.Tables[table].AsEnumerable().All(i => i.IsNull(column)))
        //                    result.Tables[table].Columns.Remove(column);
        //            }

        //            foreach (var mrow in result.Tables[table].Rows.Cast<DataRow>().ToArray())
        //            {
        //                if (string.IsNullOrEmpty(string.Join("", mrow.ItemArray).Trim()))
        //                    result.Tables[table].Rows.Remove(mrow);
        //            }
        //            table++;
        //        }
        //    }
        //    else if (option == TipoArchivos.CSV)
        //    {
        //        // Lectura de CSV a mano
        //        StreamReader reader = new StreamReader(stream);
        //        result = new DataSet();
        //        result.Tables.Add();
        //        result.Tables[0].Columns.Add();
        //        int r = 0;
        //        while (!reader.EndOfStream)
        //        {
        //            result.Tables[0].Rows.Add();
        //            string[] columns = reader.ReadLine().Split(';');
        //            if (r == 0)
        //            {
        //                // Se agregan el nombre de las columnas
        //                for (int c = 0; c < columns.Length; c++)
        //                {

        //                    result.Tables[0].Columns.Add(columns[c]);
        //                }

        //            }

        //            // Se agregan los datos a excepción del nombre de las columnas
        //            for (int c = 0; c < columns.Length - 1; c++)
        //            {
        //                result.Tables[0].Rows[r][c] = columns[c];
        //            }

        //            r++;
        //        }

        //    }
        //    else
        //        throw new Exception("Archivo no compatible");

        //    return result;
        //}

        /// <summary>
        /// Permite la importacion de hojas de calculo y archivos CSV a la base de datos
        /// </summary>
        /// <param name="stream">Stream del archivo a importar</param>
        /// <param name="option">Tipo de archivo que se va a importar</param>
        /// <param name="entitySetName">Nombre de la clase y tabla a importar</param>
        /// <returns>Devuelve el codigo y mensaje de error generado al fallar la importacion</returns>
        //public static ImportStatus ImportData(MemoryStream stream, int option, string entitySetName)
        //{
        //    try
        //    {
        //        if (stream != null)
        //        {
        //            DataSet result = null;
        //            stream.Position = 0;

        //            if (option == 1 || option == 2)
        //            {
        //                IExcelDataReader reader;
        //                if (option == TipoArchivos.CSV)
        //                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
        //                else
        //                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        //                result = reader.AsDataSet();
        //            }
        //            else if (option == 3)
        //            {
        //                StreamReader reader = new StreamReader(stream);
        //                result = Util.ToDataSet(reader);
        //            }

        //            if (option != 0 && result != null)
        //            {
        //                GCEntities db = new GCEntities();
        //                Type type = Type.GetType(db.GetType().Namespace + "." + entitySetName);
        //                int row = 1;
        //                int table = 0;

        //                while (table < result.Tables.Count)
        //                {

        //                    foreach (var column in result.Tables[table].Columns.Cast<DataColumn>().ToArray())
        //                    {
        //                        if (result.Tables[table].AsEnumerable().All(i => i.IsNull(column)))
        //                            result.Tables[table].Columns.Remove(column);
        //                        else
        //                            column.ColumnName = result.Tables[table].Rows[0][column].ToString().Trim();
        //                    }

        //                    foreach (var mrow in result.Tables[table].Rows.Cast<DataRow>().ToArray())
        //                    {
        //                        if (string.IsNullOrEmpty(string.Join("", mrow.ItemArray).Trim()))
        //                            result.Tables[table].Rows.Remove(mrow);
        //                    }

        //                    DataColumnCollection dColumns = result.Tables[table].Columns;

        //                    while (row < result.Tables[table].Rows.Count)
        //                    {
        //                        DataRow dRow = result.Tables[table].Rows[row];
        //                        object obj = Util.ToObject<dynamic>(dRow, dColumns, type);
        //                        db.AddObject(entitySetName, obj);
        //                        row++;
        //                    }
        //                    row = 0;
        //                    table++;
        //                }

        //                //sc.SaveChanges();
        //            }
        //            else
        //                return new ImportStatus { errorCode = 3, message = "Tipo de archivo incompatible." };
        //        }
        //        else
        //            return new ImportStatus { errorCode = 1, message = "El stream es nulo." };

        //        return new ImportStatus { errorCode = 0, message = "Importacion exitosa." };
        //    }
        //    catch (Exception e)
        //    {
        //        return new ImportStatus { errorCode = 2, message = e.Message };
        //    }
        //}

        /// <summary>
        /// Validar que el DataSet contenga todas las cabeceras que se indicar.
        /// </summary>
        /// <param name="columnas">Cabeceras que debera tener el data set. El Value se toma como el nombre de la cabecera.</param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static bool validarCabeceras(Dictionary<string, string> columnas, DataSet ds)
        {

            foreach (DataColumn column in ds.Tables[0].Columns)
            {
                // Con solo una cabecera que no se encuentre ya esta mal
                if (columnas.Where(i => i.Value == column.ColumnName.Trim()).Count() == 0)
                    return false;
                else
                    // Reemplazamos el nombre de la cabecera de DataSet por el nombre de propiedad
                    column.ColumnName = columnas.Where(i => i.Value == column.ColumnName).Select(i => i.Key).FirstOrDefault();
            }

            return true;
        }

        /// <summary>
        /// Convertir la informacion del DataSet en un List del tipo T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static List<T> toList<T>(DataSet ds)
        {

            List<T> result = new List<T>();
            Type type = typeof(T); // Obtenemos el tipo de T

            // Se recorre cada fila del DataSet (solo nos importa la primera tabla)
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                // Creamos una instancia dinamica al objeto
                T instance = (T)Activator.CreateInstance(type);

                // Se recorre cada columna de la fila
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    // Validamos que no este null
                    if (row[column.ColumnName] != null && row[column.ColumnName] != DBNull.Value)
                    {
                        try
                        {
                            Type prop = getType(type.GetProperty(column.ColumnName).PropertyType);
                            // Insertamos el valor a la instancia mediante reflection y además le haces cast al tipo de objeto

                            // Para casos especiales usamos los parse de los tipos complejos
                            if (prop == typeof(Guid))
                            {
                                type.GetProperty(column.ColumnName)
                                    .SetValue(instance, // El objeto donde se inserta el valor
                                             Guid.Parse(row[column.ColumnName].ToString()), // El valor a insertar
                                              null);
                            }
                            else
                            {
                                type.GetProperty(column.ColumnName)
                                    .SetValue(instance, // El objeto donde se inserta el valor
                                             Convert.ChangeType(row[column.ColumnName], prop), // El valor a insertar y el cast
                                              null); // No se para que, pero lo requiere
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

                result.Add(instance);

            }

            return result;
        }

        /// <summary>
        /// Obtener el tipo de dato cuando es de tipo nullable
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static Type getType(Type prop)
        {
            if (prop == typeof(Nullable<int>))
                return typeof(int);
            if (prop == typeof(Nullable<double>))
                return typeof(double);
            if (prop == typeof(Nullable<bool>))
                return typeof(bool);
            if (prop == typeof(Nullable<Guid>))
                return typeof(Guid);
            if (prop == typeof(Nullable<float>))
                return typeof(float);
            else
                return prop;
        }



        #endregion


        /// <summary>
        /// Genera el objecto de la clase especificada
        /// </summary>
        /// <param name="row">Fila del dataset</param>
        /// <param name="columns">Columnas del dataset</param>
        /// <param name="type">Tipo de la clase</param>
        /// <returns>Devuelve el objeto generado automaticamente</returns>
        public static T ToObject<T>(DataRow row, DataColumnCollection columns, Type type)
        {
            T item = (T)Activator.CreateInstance(type);
            foreach (DataColumn column in columns)
            {
                PropertyInfo pi = item.GetType().GetProperty(column.ColumnName);

                if (pi != null && row[column] != DBNull.Value)
                    pi.SetValue(item, Convert.ChangeType(row[column], pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? pi.PropertyType.GetGenericArguments()[0] : pi.PropertyType), new object[0]);
            }
            return item;
        }

        public static string RenderPartialToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


    }
}