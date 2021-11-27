Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class OrdenPagoMasiva
    Inherits System.Web.Services.WebService



    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function SetOP(ByVal myArray As Object, Lote As String) As String

        'Dim json As String
        'json = JS.Serialize(myArray).ToString()
        Try
            Dim oDatos As DataSet
            Dim oTabla As DataTable

            Dim oParametros As New Dictionary(Of String, Object)
            Dim JS As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim lista As New List(Of OrdenPagoMasivoClass)
            Dim Num_Lote As String

            Dim ErrorCode As Integer 'FCRUZ_GMX-10290_INCIDENCIAS BLOQUEO DE FOLIOS 
            Dim ErrorMsg As String 'FCRUZ_GMX-10290_INCIDENCIAS BLOQUEO DE FOLIOS 
            Dim ErrorMsgCta As String 'FCRUZ_GMX-10290_INCIDENCIAS VALIDA LONGITUD DE CTAS
            Dim ErrorMsgVacio As String 'JJARAMILLO_GMX-10290_INCIDENCIAS VALIDA CAMPOS OBLIGATORIOS

            ErrorMsgVacio = ""
            ErrorMsgCta = ""
            ErrorMsg = ""

            lista = New JavaScriptSerializer().ConvertToType(Of List(Of OrdenPagoMasivoClass))(myArray)

            'FCRUZ_GMX-10290_INCIDENCIAS BLOQUEO DE FOLIOS ini
            For Each OP As OrdenPagoMasivoClass In lista

                oParametros = New Dictionary(Of String, Object)

                oParametros.Add("Folio_Onbase", ValidarParametros(OP.Folio_Onbase))
                oParametros.Add("Num_Pago", ValidarParametros(OP.Num_Pago))

                Select Case OP.PagarA
                    Case "Asegurado"
                        oParametros.Add("PagarA", "7")
                    Case "Tercero"
                        oParametros.Add("PagarA", "8")
                    Case "Proveedor"
                        oParametros.Add("PagarA", "10")
                End Select
                'VZAVALETA_GMX-10290_INCIDENCIAS Validar la reserva y los asegurados
                oParametros.Add("Poliza", ValidarParametros(OP.Poliza))
                oParametros.Add("Clase_pago", ValidarParametros(OP.Cod_clas_pago))

                oParametros.Add("CodigoCliente", ValidarParametros(OP.CodigoCliente))
                oParametros.Add("RFC", ValidarParametros(OP.RFC))
                oParametros.Add("Nombre_Razon_Social", ValidarParametros(OP.Nombre_Razon_Social))

                oParametros.Add("Tipo_Cambio", ValidarParametros(OP.Tipo_Cambio))
                oParametros.Add("Reserva", ValidarParametros(OP.Reserva))
                oParametros.Add("Importe", ValidarParametros(OP.Importe))

                oDatos = Funciones.ObtenerDatos("usp_valida_folio_bloqueado", oParametros)
                ErrorCode = oDatos.Tables(0).Rows(0).Item("ID_ERROR")

                If ErrorCode <> 0 Then
                    ErrorMsg = ErrorMsg + oDatos.Tables(0).Rows(0).Item("MSG_ERROR") + "<br>"
                End If


                'JJARAMILLO_GMX-10290_INCIDENCIAS VALIDA CAMPOS OBLIGATORIOS
                If OP.PagarA = "Proveedor" Then
                    If OP.Concepto_Pago = "" Then
                        ErrorMsgVacio = ErrorMsgVacio + OP.Folio_Onbase.ToString() + "<br>"
                    End If
                Else
                    If OP.Importe = 0 Or OP.RFC = "" Or OP.CodigoCliente = "" Or OP.Nombre_Razon_Social = "" Then
                        ErrorMsgVacio = ErrorMsgVacio + OP.Folio_Onbase.ToString() + "<br>"
                    End If
                End If

                'Valida Cuentas 
                If OP.Tipo_Pago2 = "TRANSFERENCIA" Then
                    If OP.PagarA = "Proveedor" Then
                        If OP.Cuenta_Bancaria.Length < 18 AndAlso OP.Confirmar_Cuenta.Length < 18 Then
                            ErrorMsgCta = ErrorMsgCta + OP.Folio_Onbase.ToString() + "<br>"
                        End If
                    Else
                        If OP.Cuenta_Bancaria_ok.Length < 18 AndAlso OP.Confirmar_Cuenta_ok.Length < 18 Then
                            ErrorMsgCta = ErrorMsgCta + OP.Folio_Onbase.ToString() + "<br>"
                        End If
                    End If
                End If

            Next

            If ErrorMsg <> "" Then
                Throw New Exception(ErrorMsg)
            End If

            If ErrorMsgCta <> "" Then
                Throw New Exception("El número de dígitos es menor a 18 para las cuentas de los siguientes Folios: <br>" + ErrorMsgCta)
            End If
            'FCRUZ_GMX-10290_INCIDENCIAS BLOQUEO DE FOsLIOS fin

            If ErrorMsgVacio <> "" Then
                Throw New Exception("Favor de ingresar todos los campos de los folios:" + ErrorMsgVacio)
            End If

            If Lote = "0" Then
                Num_Lote = Funciones.fn_EjecutaStr("Declare @ult_lote int  EXEC sp_ult_nro_lote @ult_lote=@ult_lote out Select @ult_lote ")
            Else
                'VZAVALETA_10290_Incidencias
                Funciones.fn_EjecutaStr("Declare @ult_lote int  EXEC sp_ult_nro_lote @Action = " + Lote + "  ,@ult_lote=@ult_lote out Select @ult_lote ")

                Num_Lote = Lote
            End If



            For Each OP As OrdenPagoMasivoClass In lista

                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("Num_Lote", ValidarParametros(Num_Lote))
                oParametros.Add("Folio_Onbase", ValidarParametros(OP.Folio_Onbase))
                oParametros.Add("Num_Pago", ValidarParametros(OP.Num_Pago))
                oParametros.Add("id_Tipo_Doc", ValidarParametros(OP.Id_Tipo_Doc))
                oParametros.Add("Tipo_comprobante", ValidarParametros(OP.Tipo_comprobante))


                Select Case OP.PagarA
                    Case "Asegurado"
                        oParametros.Add("PagarA", "7")
                    Case "Tercero"
                        oParametros.Add("PagarA", "8")
                    Case "Proveedor"
                        oParametros.Add("PagarA", "10")
                End Select


                oParametros.Add("CodigoCliente", ValidarParametros(OP.CodigoCliente))
                oParametros.Add("RFC", ValidarParametros(OP.RFC))
                oParametros.Add("Nombre_Razon_Social", ValidarParametros(OP.Nombre_Razon_Social))
                oParametros.Add("Siniestro", ValidarParametros(OP.Siniestro))
                oParametros.Add("Subsiniestro", ValidarParametros(OP.Subsiniestro))
                oParametros.Add("Cod_moneda", ValidarParametros(OP.Cod_moneda))
                oParametros.Add("Moneda", ValidarParametros(OP.Moneda))
                oParametros.Add("Tipo_Cambio", ValidarParametros(OP.Tipo_Cambio))
                oParametros.Add("Reserva", ValidarParametros(OP.Reserva))
                oParametros.Add("Cod_moneda_pago", ValidarParametros(OP.Cod_moneda_pago))
                oParametros.Add("Moneda_Pago", ValidarParametros(OP.Moneda_Pago))
                oParametros.Add("Importe", ValidarParametros(OP.Importe))
                oParametros.Add("Deducible", ValidarParametros(OP.Deducible))
                oParametros.Add("Importe_concepto", ValidarParametros(OP.Importe_concepto))
                oParametros.Add("Concepto_Facturado", ValidarParametros(OP.Concepto_Factura))
                oParametros.Add("Concepto_Pago", ValidarParametros(OP.Cod_concepto_pago))
                oParametros.Add("Clase_pago", ValidarParametros(OP.Cod_clas_pago))
                oParametros.Add("Tipo_Pago", ValidarParametros(OP.Cod_tipo_pago))
                oParametros.Add("Concepto2", ValidarParametros(OP.Concepto2))
                oParametros.Add("Tipo_Pago2", ValidarParametros(OP.Tipo_Pago2))
                oParametros.Add("Folio_Onbase_cuenta", ValidarParametros(OP.Folio_Onbase_cuenta))

                If OP.PagarA = "Proveedor" Then
                    oParametros.Add("Cuenta_Bancaria", ValidarParametros(OP.Cuenta_Bancaria))
                    oParametros.Add("Confirmar_Cuenta", ValidarParametros(OP.Confirmar_Cuenta))
                Else
                    oParametros.Add("Cuenta_Bancaria", ValidarParametros(OP.Cuenta_Bancaria_ok))
                    oParametros.Add("Confirmar_Cuenta", ValidarParametros(OP.Confirmar_Cuenta_ok))
                End If
                oParametros.Add("Solicitante", ValidarParametros(OP.Solicitante))
                oParametros.Add("Notas", ValidarParametros(OP.Notas))
                oParametros.Add("Observaciones", ValidarParametros(OP.Observaciones))
                oParametros.Add("id_persona", ValidarParametros(OP.Id_Persona))
                oParametros.Add("CodigoSucursal", ValidarParametros(OP.CodigoSucursal))
                oParametros.Add("TipoMovimiento", ValidarParametros(OP.TipoMovimiento))
                oParametros.Add("VariasFacturas", ValidarParametros(OP.VariasFacturas))
                oParametros.Add("Ramo", ValidarParametros(OP.Ramo))
                oParametros.Add("SubRamo", ValidarParametros(OP.SubRamo))
                oParametros.Add("ID_TipoComprobante", ValidarParametros(OP.ID_TipoComprobante))
                oParametros.Add("NumeroComprobante", ValidarParametros(OP.NumeroComprobante))

                OP.FechaComprobante = Funciones.FormatearFecha(OP.FechaComprobante, Funciones.enumFormatoFecha.YYYYMMDD)
                OP.FechaIngreso = Funciones.FormatearFecha(OP.FechaIngreso, Funciones.enumFormatoFecha.YYYYMMDD)
                OP.Fec_pago = Funciones.FormatearFecha(OP.Fec_pago, Funciones.enumFormatoFecha.YYYYMMDD)

                oParametros.Add("FechaComprobante", ValidarParametros(OP.FechaComprobante))
                oParametros.Add("CodTipoStro", ValidarParametros(OP.CodTipoStro))
                oParametros.Add("CodigoOrigenPago", ValidarParametros(OP.CodigoOrigenPago))
                oParametros.Add("FechaIngreso", ValidarParametros(OP.FechaIngreso))
                oParametros.Add("CodigoBancoTransferencia", ValidarParametros(OP.CodigoBancoTransferencia))



                oParametros.Add("IdSiniestro", ValidarParametros(OP.IdSiniestro))
                oParametros.Add("CodigoTercero", ValidarParametros(OP.CodigoTercero))
                oParametros.Add("Subtotal", ValidarParametros(OP.Importe))
                oParametros.Add("Iva", ValidarParametros(OP.Iva))
                oParametros.Add("Total", ValidarParametros(OP.Total))
                oParametros.Add("Retencion", ValidarParametros(OP.Retencion))
                oParametros.Add("CodItem", ValidarParametros(OP.CodItem))
                oParametros.Add("CodIndCob", ValidarParametros(OP.CodIndCob))
                oParametros.Add("NumeroCorrelaEstim", ValidarParametros(OP.NumeroCorrelaEstim))
                oParametros.Add("NumeroCorrelaPagos", ValidarParametros(OP.NumeroCorrelaPagos))
                oParametros.Add("SnCondusef", ValidarParametros(OP.SnCondusef))
                oParametros.Add("NumeroOficioCondusef", ValidarParametros(OP.NumeroOficioCondusef))
                oParametros.Add("TipoPagoDetalle", ValidarParametros(OP.TipoPagoDetalle))
                oParametros.Add("Cod_objeto", ValidarParametros(OP.Cod_objeto))
                oParametros.Add("Poliza", ValidarParametros(OP.Poliza))
                oParametros.Add("Fec_pago", ValidarParametros(OP.Fec_pago))










                Funciones.ObtenerDatos("sp_grabar_temporal_OP", oParametros)

            Next





            Return Num_Lote
        Catch ex As Exception
            Return ex.Message
        End Try



    End Function

    Private Function ValidarParametros(txt_campo As String) As String
        Try
            If IsNothing(txt_campo) Then
                txt_campo = Nothing
                Return Nothing
            End If



            If txt_campo.Length = 0 Then
                txt_campo = Nothing
                Return Nothing
            End If



            If txt_campo.Length > 0 Then
                Return txt_campo
            End If



            Return txt_campo
        Catch ex As Exception
            Return Nothing



        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function RecuperaLote(ByVal NumLote As String) As String


        Dim cadena As String
        Dim datos() As String
        cadena = HttpContext.Current.User.Identity.Name.ToString()
        datos = cadena.Split("|")



        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim serializer As New JavaScriptSerializer

        Dim lstOp As New List(Of OrdenPagoMasivoClass)
        Dim OP As New OrdenPagoMasivoClass
        Dim ID As Int32



        ID = 1

        serializer.MaxJsonLength = 500000000

        Try
            oParametros.Add("Num_Lote", NumLote)


            oDatos = Funciones.ObtenerDatos("sp_recupera_lote_OP_Masivo", oParametros)
            oTabla = oDatos.Tables(0)

            For Each row As DataRow In oTabla.Rows
                OP = New OrdenPagoMasivoClass

                'OP.Folio_Onbase = "<a href=""VisordeContenido.aspx ""  target=""_blank""><i class=""fa fa-newspaper-o""></i>&nbsp; " + row("Folio_Onbase").ToString() + "</a>"
                'VZAVALETA_GMX-10290_INCIDENCIAS Se agrega como parametro PagarA
                OP.Folio_Onbase = Webservices(row("Folio_Onbase").ToString(), row("PagarA").ToString())
                OP.Num_Pago = row("Num_Pago").ToString()
                OP.Tipo_comprobante = row("Tipo_comprobante").ToString()
                Select Case row("PagarA").ToString()
                    Case "7"
                        OP.PagarA = "Asegurado"
                    Case "8"
                        OP.PagarA = "Tercero"
                    Case "10"
                        OP.PagarA = "Proveedor"
                End Select

                OP.CodigoCliente = row("CodigoCliente").ToString()
                OP.RFC = row("RFC").ToString()
                OP.Nombre_Razon_Social = row("Nombre_Razon_Social").ToString()
                OP.Siniestro = row("Siniestro").ToString()
                OP.Subsiniestro = row("Subsiniestro").ToString()
                OP.Moneda = row("Moneda").ToString()
                OP.Tipo_Cambio = row("Tipo_Cambio").ToString()
                OP.Reserva = row("Reserva").ToString()
                OP.Moneda_Pago = row("Moneda_Pago").ToString()
                OP.Importe = row("Importe").ToString()
                OP.Deducible = row("Deducible").ToString()
                OP.Importe_concepto = row("Importe_concepto").ToString()
                OP.Concepto_Factura = row("Concepto_Facturado").ToString()
                OP.Concepto_Pago = row("Concepto_Pago").ToString()
                OP.Clase_pago = row("Clase_pago").ToString()
                OP.Tipo_Pago = row("Tipo_Pago").ToString()
                OP.Concepto2 = row("Concepto2").ToString()
                OP.Tipo_Pago2 = row("Tipo_Pago2").ToString()
                OP.Folio_Onbase_cuenta = Webservices(row("Folio_Onbase_cuenta").ToString(), row("PagarA").ToString())
                OP.Cuenta_Bancaria = row("Cuenta_Bancaria").ToString()
                OP.Confirmar_Cuenta = row("Confirmar_Cuenta").ToString()
                OP.Cuenta_Bancaria_ok = row("Cuenta_Bancaria").ToString()
                OP.Confirmar_Cuenta_ok = row("Confirmar_Cuenta").ToString()
                OP.Solicitante = row("Solicitante").ToString()
                OP.Notas = row("Notas").ToString()
                OP.Observaciones = row("Observaciones").ToString()
                OP.Id_Tipo_Doc = row("Id_Tipo_Doc").ToString()
                OP.Cod_moneda = row("Cod_moneda").ToString()
                OP.Cod_moneda_pago = row("Cod_moneda_pago").ToString()
                OP.FolioOnbaseHidden = row("Folio_Onbase").ToString()
                OP.Folio_Onbase_cuentaHidden = row("Folio_Onbase_cuenta").ToString()
                OP.Id_Persona = row("id_persona").ToString()
                OP.CodigoSucursal = row("CodigoSucursal").ToString()
                OP.TipoMovimiento = row("TipoMovimiento").ToString()
                OP.VariasFacturas = row("VariasFacturas").ToString()
                OP.Ramo = row("Ramo").ToString()
                OP.SubRamo = row("SubRamo").ToString()
                OP.ID_TipoComprobante = row("ID_TipoComprobante").ToString()
                OP.NumeroComprobante = row("NumeroComprobante").ToString()
                OP.FechaComprobante = row("FechaComprobante").ToString()
                OP.CodTipoStro = row("CodTipoStro").ToString()
                OP.CodigoOrigenPago = row("CodigoOrigenPago").ToString()
                OP.FechaIngreso = row("FechaIngreso").ToString()
                OP.CodigoBancoTransferencia = row("CodigoBancoTransferencia").ToString()


                OP.IdSiniestro = row("IdSiniestro").ToString()
                OP.CodigoTercero = row("CodigoTercero").ToString()
                OP.Subtotal = row("Subtotal").ToString()
                OP.Iva = row("Iva").ToString()
                OP.Total = row("Total").ToString()
                OP.Retencion = row("Retencion").ToString()
                OP.CodItem = row("CodItem").ToString()
                OP.CodIndCob = row("CodIndCob").ToString()
                OP.NumeroCorrelaEstim = row("NumeroCorrelaEstim").ToString()
                OP.NumeroCorrelaPagos = row("NumeroCorrelaPagos").ToString()
                OP.SnCondusef = row("SnCondusef").ToString()
                OP.NumeroOficioCondusef = row("NumeroOficioCondusef").ToString()
                OP.TipoPagoDetalle = row("TipoPagoDetalle").ToString()
                OP.Cod_objeto = row("Cod_objeto").ToString()

                OP.Cod_concepto_pago = row("Cod_concepto_pago").ToString()
                OP.Cod_clas_pago = row("Cod_clas_pago").ToString()
                OP.Cod_tipo_pago = row("Cod_tipo_pago").ToString()
                OP.Poliza = row("Poliza").ToString()
                OP.Fec_pago = row("fec_pago").ToString()
                OP.AltaTercero = "<input class=""btn btn-primary"" type=""button""  id=""" + ID.ToString() + "_row"" OnClick=""Terceros(" + ID.ToString() + ")"" value=""..."">"
                lstOp.Add(OP)

                ID = ID + 1

            Next



            Return serializer.Serialize(lstOp)
        Catch ex As Exception
            Return ""
        End Try



    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function ConceptoPago() As String

        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim serializer As New JavaScriptSerializer


        Dim OP As New OrdenPagoMasivoClass






        serializer.MaxJsonLength = 500000000


        oParametros.Add("TipoUsuario", "10")
        oParametros.Add("ClasePago", "26")
        oParametros.Add("CodigoPres", "52260")


        'oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPago_stro", oParametros)
        oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPago_stro_OPWEB", oParametros) 'FJCP_10290_CC

        oTabla = oDatos.Tables(0)

        Return JsonConvert.SerializeObject(oTabla)
        'Return " 1:'Diego Avila', 2:'Julia Pazmino', 3:'Vinicio Coello'"



    End Function



    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function Apoyo() As String





        Return ""

    End Function



    Public Function Webservices(Folio_Onbase As String, Optional PagarA As String = "") As String


        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim url As String
        Dim salida As String
        url = ""
        oParametros.Add("strCatalogo", "WebServices")
        oParametros.Add("Condicion", PagarA) 'VZAVALETA_GMX-10290_INCIDENCIAS
        oDatos = Funciones.ObtenerDatos("[sp_Catalogos_OPMasivas]", oParametros)
        oTabla = oDatos.Tables(0)

        For Each row As DataRow In oTabla.Rows
            url = row("url").ToString()
            url = url.Replace("@Folio", Folio_Onbase)

        Next
        salida = "<a href=""" + url + """  target=""_blank""><i class=""fa fa-newspaper-o""></i>&nbsp; " + Folio_Onbase + "</a>"
        Return salida

    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function RecuperaTercero(ByVal Nombre As String, ByVal RFC As String, ByVal Codigo As String) As String



        Dim oDatos As DataSet
            Dim oTabla As DataTable
            Dim oParametros As New Dictionary(Of String, Object)
            Dim serializer As New JavaScriptSerializer


        Try


            serializer.MaxJsonLength = 500000000
            oParametros.Add("strCatalogo", "Tercero")

            If Nombre.Length > 0 Then
                oParametros.Add("Condicion", Nombre)
            End If

            If RFC.Length > 0 Then
                oParametros.Add("rfc", RFC)
            End If

            If Codigo.Length > 0 Then
                oParametros.Add("cod_tercero", Codigo)
            End If






            oDatos = Funciones.ObtenerDatos("[sp_Catalogos_OPMasivas]", oParametros)

            oTabla = oDatos.Tables(0)

            Return JsonConvert.SerializeObject(oTabla)
        Catch ex As Exception
            Return JsonConvert.SerializeObject(oTabla)
        End Try


    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function folioOnbaseBloqueado(codUsuario As String, bloqueo As Integer) As String

        Dim oParametros As New Dictionary(Of String, Object)
        Dim serializer As New JavaScriptSerializer
        Dim OP As New OrdenPagoMasivoClass

        serializer.MaxJsonLength = 500000000

        oParametros.Add("Accion", 3)
        oParametros.Add("folioOnbase", 999)
        oParametros.Add("cod_usuario", codUsuario)

        If bloqueo <> 0 Then
            Funciones.ObtenerDatos("usp_bloqueoFolioOnbase_stro", oParametros)
        End If


        Return Nothing
    End Function




    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function RecuperarClasePago(ByVal Cpto_pago As String, ByVal sn_prestador As String) As String



        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim serializer As New JavaScriptSerializer


        Try


            serializer.MaxJsonLength = 500000000
            oParametros.Add("cod_Concepto", Cpto_pago)

            oParametros.Add("sn_prestador", sn_prestador)


            oDatos = Funciones.ObtenerDatos("usp_ObtenerClasePagoMasivo_stro", oParametros)

            oTabla = oDatos.Tables(0)

            Return JsonConvert.SerializeObject(oTabla)
        Catch ex As Exception
            Return JsonConvert.SerializeObject(oTabla)
        End Try


    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function RecuperarCtasDepend(ByVal CodCliente As String, ByVal FolioOnbase As String) As String
        'VZAVALETA_10290_CC7
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim serializer As New JavaScriptSerializer
        Try
            serializer.MaxJsonLength = 500000000
            oParametros.Add("Folio_OnBase", FolioOnbase)
            oParametros.Add("cod_cliente", CodCliente)

            oDatos = Funciones.ObtenerDatos("sp_Recuperar_Ctas_Depend", oParametros)
            oTabla = oDatos.Tables(0)

            Return JsonConvert.SerializeObject(oTabla)
        Catch ex As Exception
            Return JsonConvert.SerializeObject(oTabla)
        End Try


    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function Export_xls(ByVal myArray As Object, Lote As String) As String
        Dim oParametros As New Dictionary(Of String, Object)
        Dim JS As New System.Web.Script.Serialization.JavaScriptSerializer
        Dim lista As New List(Of OrdenPagoMasivoClass)
        Dim Num_Lote As String


        Try



            lista = New JavaScriptSerializer().ConvertToType(Of List(Of OrdenPagoMasivoClass))(myArray)


            Num_Lote = Funciones.fn_Ejecuta("DECLARE @ult_lote INT EXEC spiu_tvarias_ult_nro 0, nro_lote_aux_masivo_stro, @ult_nro = @ult_lote OUTPUT SELECT @ult_lote ")



            For Each OP As OrdenPagoMasivoClass In lista

                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("Accion", 1)
                oParametros.Add("Num_Lote", ValidarParametros(Num_Lote))
                oParametros.Add("Folio_Onbase", ValidarParametros(OP.Folio_Onbase))
                oParametros.Add("Num_Pago", ValidarParametros(OP.Num_Pago))
                oParametros.Add("id_Tipo_Doc", ValidarParametros(OP.Id_Tipo_Doc))
                oParametros.Add("Tipo_comprobante", ValidarParametros(OP.Tipo_comprobante))


                Select Case OP.PagarA
                    Case "Asegurado"
                        oParametros.Add("PagarA", "7")
                    Case "Tercero"
                        oParametros.Add("PagarA", "8")
                    Case "Proveedor"
                        oParametros.Add("PagarA", "10")
                End Select


                oParametros.Add("CodigoCliente", ValidarParametros(OP.CodigoCliente))
                oParametros.Add("RFC", ValidarParametros(OP.RFC))
                oParametros.Add("Nombre_Razon_Social", ValidarParametros(OP.Nombre_Razon_Social))
                oParametros.Add("Siniestro", ValidarParametros(OP.Siniestro))
                oParametros.Add("Subsiniestro", ValidarParametros(OP.Subsiniestro))
                oParametros.Add("Cod_moneda", ValidarParametros(OP.Cod_moneda))
                oParametros.Add("Moneda", ValidarParametros(OP.Moneda))
                oParametros.Add("Tipo_Cambio", ValidarParametros(OP.Tipo_Cambio))
                oParametros.Add("Reserva", ValidarParametros(OP.Reserva))
                oParametros.Add("Cod_moneda_pago", ValidarParametros(OP.Cod_moneda_pago))
                oParametros.Add("Moneda_Pago", ValidarParametros(OP.Moneda_Pago))
                oParametros.Add("Importe", ValidarParametros(OP.Importe))
                oParametros.Add("Deducible", ValidarParametros(OP.Deducible))
                oParametros.Add("Importe_concepto", ValidarParametros(OP.Importe_concepto))
                oParametros.Add("Concepto_Facturado", ValidarParametros(OP.Concepto_Factura))
                oParametros.Add("Concepto_Pago", ValidarParametros(OP.Cod_concepto_pago))
                oParametros.Add("Clase_pago", ValidarParametros(OP.Cod_clas_pago))
                oParametros.Add("Tipo_Pago", ValidarParametros(OP.Cod_tipo_pago))
                oParametros.Add("Concepto2", ValidarParametros(OP.Concepto2))
                oParametros.Add("Tipo_Pago2", ValidarParametros(OP.Tipo_Pago2))
                oParametros.Add("Folio_Onbase_cuenta", ValidarParametros(OP.Folio_Onbase_cuenta))

                If OP.PagarA = "Proveedor" Then
                    oParametros.Add("Cuenta_Bancaria", ValidarParametros(OP.Cuenta_Bancaria))
                    oParametros.Add("Confirmar_Cuenta", ValidarParametros(OP.Confirmar_Cuenta))
                Else
                    oParametros.Add("Cuenta_Bancaria", ValidarParametros(OP.Cuenta_Bancaria_ok))
                    oParametros.Add("Confirmar_Cuenta", ValidarParametros(OP.Confirmar_Cuenta_ok))
                End If
                oParametros.Add("Solicitante", ValidarParametros(OP.Solicitante))
                oParametros.Add("Notas", ValidarParametros(OP.Notas))
                oParametros.Add("Observaciones", ValidarParametros(OP.Observaciones))
                oParametros.Add("id_persona", ValidarParametros(OP.Id_Persona))
                oParametros.Add("CodigoSucursal", ValidarParametros(OP.CodigoSucursal))
                oParametros.Add("TipoMovimiento", ValidarParametros(OP.TipoMovimiento))
                oParametros.Add("VariasFacturas", ValidarParametros(OP.VariasFacturas))
                oParametros.Add("Ramo", ValidarParametros(OP.Ramo))
                oParametros.Add("SubRamo", ValidarParametros(OP.SubRamo))
                oParametros.Add("ID_TipoComprobante", ValidarParametros(OP.ID_TipoComprobante))
                oParametros.Add("NumeroComprobante", ValidarParametros(OP.NumeroComprobante))

                OP.FechaComprobante = Funciones.FormatearFecha(OP.FechaComprobante, Funciones.enumFormatoFecha.YYYYMMDD)
                OP.FechaIngreso = Funciones.FormatearFecha(OP.FechaIngreso, Funciones.enumFormatoFecha.YYYYMMDD)
                OP.Fec_pago = Funciones.FormatearFecha(OP.Fec_pago, Funciones.enumFormatoFecha.YYYYMMDD)

                oParametros.Add("FechaComprobante", ValidarParametros(OP.FechaComprobante))
                oParametros.Add("CodTipoStro", ValidarParametros(OP.CodTipoStro))
                oParametros.Add("CodigoOrigenPago", ValidarParametros(OP.CodigoOrigenPago))
                oParametros.Add("FechaIngreso", ValidarParametros(OP.FechaIngreso))
                oParametros.Add("CodigoBancoTransferencia", ValidarParametros(OP.CodigoBancoTransferencia))

                oParametros.Add("IdSiniestro", ValidarParametros(OP.IdSiniestro))
                oParametros.Add("CodigoTercero", ValidarParametros(OP.CodigoTercero))
                oParametros.Add("Subtotal", ValidarParametros(OP.Importe))
                oParametros.Add("Iva", ValidarParametros(OP.Iva))
                oParametros.Add("Total", ValidarParametros(OP.Total))
                oParametros.Add("Retencion", ValidarParametros(OP.Retencion))
                oParametros.Add("CodItem", ValidarParametros(OP.CodItem))
                oParametros.Add("CodIndCob", ValidarParametros(OP.CodIndCob))
                oParametros.Add("NumeroCorrelaEstim", ValidarParametros(OP.NumeroCorrelaEstim))
                oParametros.Add("NumeroCorrelaPagos", ValidarParametros(OP.NumeroCorrelaPagos))
                oParametros.Add("SnCondusef", ValidarParametros(OP.SnCondusef))
                oParametros.Add("NumeroOficioCondusef", ValidarParametros(OP.NumeroOficioCondusef))
                oParametros.Add("TipoPagoDetalle", ValidarParametros(OP.TipoPagoDetalle))
                oParametros.Add("Cod_objeto", ValidarParametros(OP.Cod_objeto))
                oParametros.Add("Poliza", ValidarParametros(OP.Poliza))
                oParametros.Add("Fec_pago", ValidarParametros(OP.Fec_pago))

                Funciones.ObtenerDatos("sp_grabar_temporal_xls_OP", oParametros)
            Next


            Return Num_Lote

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

End Class