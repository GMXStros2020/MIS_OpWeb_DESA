<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="OrdenPagoFondos.aspx.vb" Inherits="Siniestros_OrdenPago" %>

<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="1|0" />
    <script src="../Scripts/Siniestros/OrdenPagoFondos.js"></script>
    <style>
        .table > tbody > tr > td {
            padding: 0px;
        }

            .table > tbody > tr > td > input, .table > tbody > tr > td > select {
                border: 0px;
            }

        .chkeliminar {
            border: 0px;
        }

        .table > tbody > tr > th {
            padding: 2px;
        }

        .zona-principal {
            padding-right: 10px;
        }

        .btnEjemplo {
            margin-top: 20px;
        }

        @media (max-width:1080px) {

            .zona-fechas {
                display: none;
            }

            .zona-form {
                padding-left: 20px;
            }
        }
    </style>
    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <asp:UpdatePanel runat="server" ID="upGenerales">
            <ContentTemplate>
                <div class="cuadro-titulo panel-encabezado">
                    <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana1" class="contraer" />
                    <input type="image" src="../Images/expander_mini_inv.png" id="exVentana1" class="expandir" />
                    Solicitud de pago Fondos
                </div>
                <asp:HiddenField runat="server" ID="hid_Operacion" Value="0" />
                <div class="panel-contenido ventana1">
                    <div class="row">
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Pagar a</asp:Label>
                            <asp:DropDownList AutoPostBack="true" ID="cmbTipoUsuario" runat="server" ClientIDMode="Static" CssClass="estandar-control tipoUsuario Tablero" OnSelectedIndexChanged="cmb_SelectedIndexChanged">
                                <asp:ListItem Value="7">ASEGURADO</asp:ListItem>
                                <%--A--%>
                                <asp:ListItem Value="8">TERCERO</asp:ListItem>
                                <%--T--%>
                                <asp:ListItem Value="10">PROVEEDOR</asp:ListItem>
                                <%--P--%>
                            </asp:DropDownList>
                        </div>
                        <div runat="server">
                            <%--FJCP Folio OnBase--%>
                            <%--                            <div class="form-group col-md-2">
                                <asp:Label ID="lblObBase" runat="server" class="etiqueta-control">Folio Onbase</asp:Label>
                                <asp:TextBox AutoPostBack="True" OnBlur="__doPostBack(this.id, '');" ID="txtOnBase" runat="server" Text='<%# Eval("Onbase") %>' OnTextChanged="txt_TextChanged" CssClass="estandar-control onbase Tablero Centro" placeholder="Folio Onbase"></asp:TextBox>
				</div>--%>
                            <div class="form-group col-md-2" style="padding: 0px; margin: 0px">
                                <div class="col-md-8">
                                    <asp:Label ID="lblObBase" runat="server" class="etiqueta-control">Folio Onbase</asp:Label>
                                    <asp:TextBox AutoPostBack="True" OnBlur="__doPostBack(this.id, '');" ID="txtOnBase" runat="server" Text='<%# Eval("Onbase") %>' OnTextChanged="txt_TextChanged" CssClass="estandar-control onbase Tablero Centro" placeholder="Folio Onbase" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-4" style="text-align: left; padding: 0px; margin: 0px">
                                    <asp:Label ID="Label1" runat="server" class="etiqueta-control" Text="......." ForeColor="White" Width="100%"></asp:Label>
                                    <%--<a id="linkOnBase" runat="server" href="#" target="_blank" class="btn btn-primary btn-xs" style="background-color: #003A5D; height: 18px; vertical-align: top; padding: 1px; font-size: 11px; width: 75%">Onbase</a>--%>
                                    <a id="linkOnBase" runat="server" href="#" target="_blank">Onbase</a>
                                </div>
                            </div>
                        </div>

                        <div class="form-group col-md-1">
                            <asp:Label runat="server" class="etiqueta-control">Num. Pago</asp:Label>
                            <asp:DropDownList AutoPostBack="true" ID="cmbNumPago" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                            </asp:DropDownList>
                        </div>


                        <div class="form-group col-md-1">
                            <asp:Label runat="server" class="etiqueta-control">Siniestro</asp:Label>
                            <asp:TextBox AutoPostBack="True" OnBlur="__doPostBack(this.id, '');" OnTextChanged="txt_TextChanged" ID="txtSiniestro" runat="server" Text='<%# Eval("Siniestro") %>' CssClass="estandar-control siniestro Tablero Centro" placeholder="Número de siniestro"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Subsiniestro</asp:Label>
                            <asp:DropDownList ID="cmbSubsiniestro" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero Centro"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Póliza</asp:Label>
                            <asp:TextBox ID="txtPoliza" runat="server" CssClass="estandar-control poliza Tablero Centro" ReadOnly="true" placeholder="Número de póliza"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Moneda de la póliza</asp:Label>
                            <asp:TextBox ID="txtMonedaPoliza" runat="server" Text='<%# Eval("Moneda") %>' CssClass="estandar-control onbase Tablero Centro" ReadOnly="true" placeholder="Moneda de la póliza"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div id="RFC" runat="server">
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control">RFC</asp:Label>
                                <asp:TextBox AutoPostBack="True" ID="txtRFC" runat="server" Text='<%# Eval("OcultaCampo1") %>' CssClass="estandar-control RFC Tablero Centro" placeholder="RFC"></asp:TextBox>
                                <%--<asp:TextBox AutoPostBack="True" ID="TextBox1" runat="server" Text='<%# Eval("OcultaCampo1") %>' CssClass="estandar-control RFC Tablero Centro" OnTextChanged="txt_TextChanged" placeholder="RFC"></asp:TextBox>--%>
                            </div>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Código</asp:Label>
                            <asp:TextBox ID="txtCodigoBeneficiario_stro" runat="server" CssClass="estandar-control Tablero Centro" ReadOnly="true" placeholder="Código de tercero, proveedor o asegurado"></asp:TextBox>
                        </div>
                        <%--<div class="form-group col-md-4">
                            <asp:Label runat="server" class="etiqueta-control">Nombre / Razón social</asp:Label>
                            <asp:TextBox AutoPostBack="True" ID="txtBeneficiario_stro" runat="server" Text='<%# Eval("Nombre") %>' CssClass="estandar-control Tablero Centro" autocomplete="off" placeholder="Nombre o razón social"></asp:TextBox>
                        </div>
			                            <%--FJC Mejoras Nombre y Razón Social -Inicio--%>
                        <div class="form-group col-md-4" style="padding: 0px; margin: 0px">
                            <div class="col-md-11">
                                <asp:Label runat="server" class="etiqueta-control">Nombre / Razón social</asp:Label>
                                <asp:TextBox AutoPostBack="True" ID="txtBeneficiario_stro" runat="server" Text='<%# Eval("Nombre") %>' CssClass="estandar-control Tablero Centro" autocomplete="off" placeholder="Nombre o razón social" Width="110%"></asp:TextBox>

                            </div>
                            <div class="col-md-1" style="text-align: left; padding: 0px; margin: 0px">
                                <asp:Label ID="lblNvoTercero" runat="server" class="etiqueta-control" Text="....." ForeColor="White" Visible="false"></asp:Label>
                                <asp:LinkButton ID="btnNvoTercero" runat="server" class="btn btn-primary btn-xs" Style="background-color: #003A5D; height: 19px; width: 22px; vertical-align: top; padding: 0px; font-size: 11px" Visible="false">
                                    <span>
                                        <img class="btn-añadir"/>  
                                    </span>
                                </asp:LinkButton>

                            </div>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Moneda de pago</asp:Label>
                            <asp:DropDownList AutoPostBack="True" ID="cmbMonedaPago" Enabled="true" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero Centro" OnSelectedIndexChanged="CalcularTotales">
                                <asp:ListItem Value="0">NACIONAL</asp:ListItem>
                                <asp:ListItem Value="1">DOLAR AMERICANO</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <%--<div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Tipo de cambio</asp:Label>                            
                            <asp:TextBox ID="txtTipoCambio" runat="server" CssClass="estandar-control Tablero Centro" placeholder="1.00"></asp:TextBox>                            
                        </div>--%>

                        <%--JLC Mejoras Tipo de Cambio Pactado - Ini--%>
                        <%-- <div class="form-group col-md-2" style="padding: 0px; margin: 0px">
                            <div class="col-md-8">
                                <asp:Label runat="server" class="etiqueta-control">Tipo de cambio</asp:Label>
                                <asp:TextBox ID="txtTipoCambio" runat="server" CssClass="estandar-control Tablero Centro" placeholder="1.00"></asp:TextBox>
                            </div>
                            <div class="col-md-4" style="text-align: left; padding: 0px; margin: 0px">
                                <asp:Label ID="Label2" runat="server" class="etiqueta-control" Text="" ForeColor="White" Width="60%"></asp:Label>
                                <a href="#" class="btn btn-primary btn-xs" data-toggle="modal" data-target="#Modal" style="background-color: #003A5D; height: 18px; vertical-align: top; padding: 1px; font-size: 8px; width: 75%">Tipo de Cambio</a>
                            </div>
                        </div>--%>
                        <div class="form-group col-md-2" style="padding: 0px; margin: 0px">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Label runat="server" class="col-md-12 etiqueta-control">Tipo de cambio</asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtTipoCambio" runat="server" CssClass=" col-md-5 estandar-control Tablero Centro" placeholder="1.00" Width="95%"></asp:TextBox>
                                    </div>
                                    <div class="col-md-7">

                                        <a href="#" class="btn btn-primary btn-xs" data-toggle="modal" data-target="#Modal" style="background-color: #003A5D; height: 18px; vertical-align: top; padding: 1px; font-size: 8px; width: 95%">Tipo de Cambio</a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%--JLC Mejoras Tipo de Cambio Pactado - Fin--%>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Fecha de registro</asp:Label>
                            <asp:TextBox ID="txtFechaRegistro" runat="server" CssClass="estandar-control Tablero Centro" placeholder="DD/MM/YYYY" autocomplete="off" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Fec estim de pago</asp:Label>
                            <asp:TextBox ID="txtFechaEstimadaPago" AutoPostBack="True" OnTextChanged="txt_TextChanged" runat="server" CssClass="estandar-control fechadepago Tablero Fecha Centro" autocomplete="off" placeholder="DD/MM/YYYY"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Fecha contable</asp:Label>
                            <asp:TextBox ID="txtFechaContable" OnTextChanged="txt_TextChanged" runat="server" CssClass="estandar-control fechaContable Tablero Centro" placeholder="DD/MM/YYYY" autocomplete="off" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div id="Comprobantes">
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control">Tipo comprobante</asp:Label>
                                <asp:DropDownList ID="cmbTipoComprobante" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero Centro" readonly="true"></asp:DropDownList>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control">Núm comprobante</asp:Label>
                                <asp:TextBox ID="txtNumeroComprobante" runat="server" Text='<%# Eval("NumeroComprobante") %>' CssClass="estandar-control Tablero Centro"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control">Fecha comprobante</asp:Label>
                                <asp:TextBox ID="txtFechaComprobante" runat="server" Text='<%# Eval("FechaComprobante") %>' CssClass="estandar-control Tablero Fecha Centro" autocomplete="off"></asp:TextBox>

                                <asp:TextBox ID="txt_clase" runat="server" hidden="true"></asp:TextBox>
                            </div>
                        </div>

                    </div>
                    <div id="pnlProveedor" class="row" runat="server">
                        <div class="form-group col-md-2">
                            <div class="form-check Centrado">
                                <asp:CheckBox runat="server" ID="chkVariasFacturas" Text="Varios documentos" CssClass="etiqueta-control" />
                            </div>
                        </div>
                        <div class="form-group col-md-2">
                            <div class="form-check Centrado">
                                <asp:CheckBox runat="server" ID="chkVariosConceptos" Text="Varios conceptos" Visible="true" CssClass="etiqueta-control" />
                            </div>
                        </div>
                        <div class="form-group col-md-2">
                            <div class="form-check Centrado">
                                <asp:CheckBox runat="server" ID="chkFondosSinIVA" Text="Fondos Sin IVA" CssClass="etiqueta-control hidden" />
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%; text-align: left">
                        <asp:LinkButton ID="btnAgregarFila" runat="server" class="btn btn-primary btn-xs" Style="background-color: #003A5D;" OnClientClick="return ValidarBeneficiario();">
                            <span>
                                <img class="btn-añadir"/> AÑADIR REGISTRO
                            </span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnQuitarFila" runat="server" class="btn btn-danger btn-xs">
                            <span>
                                <img class="btn-eliminar"/> QUITAR REGISTRO
                            </span>
                        </asp:LinkButton>
                    </div>
                    <div class="row">
                        <div class="panel-contenido ventana2">
                            <div class="panel-subcontenido">
                                <asp:Panel runat="server" Width="100%" Height="100px" ScrollBars="Both">
                                    <asp:GridView ID="grd" runat="server" DataKeyNames="Siniestro" AutoGenerateColumns="false"
                                        CssClass="table grid-view" HeaderStyle-CssClass="header" AlternatingRowStyle-CssClass="altern"
                                        OnRowDeleting="grd_RowDeleting" OnRowDataBound="grd_RowDataBound"
                                        GridLines="Vertical" ShowHeaderWhenEmpty="true">
                                        <Columns>
                                            <asp:BoundField DataField="RowNumber" HeaderText="Número fila" Visible="false" />
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="eliminar" runat="server" CssClass="estandar-control chkeliminar" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Factura">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Factura") %>' CssClass="estandar-control factura"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Siniestro">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Siniestro") %>' CssClass="estandar-control"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Subsiniestro" Visible="false">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Subsiniestro") %>' CssClass="estandar-control"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Estimacion" Visible="false">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Estimacion") %>' CssClass="estandar-control Estimacion"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                               
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Reserva" Visible="false">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Reserva") %>' CssClass="estandar-control Reserva" style="text-align:right"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="ImportePagos" Visible="false">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("ImportePagos") %>' CssClass="estandar-control ImportePagos"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Póliza" Visible="false">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Poliza") %>' CssClass="estandar-control poliza"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Moneda">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("Moneda") %>' CssClass="estandar-control"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Concepto de pago">
                                                <ItemTemplate>
                                                    <asp:DropDownList AutoPostBack="True" runat="server" ClientIDMode="Static" CssClass="estandar-control concepto_pago" OnSelectedIndexChanged="cmb_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Clase de pago">
                                                <ItemTemplate>
                                                    <asp:DropDownList AutoPostBack="True" runat="server" ClientIDMode="Static" Enabled="false" CssClass="estandar-control clase_pago">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Pago">
                                                <ItemTemplate>
                                                    <asp:TextBox AutoPostBack="True" OnSelectedIndexChanged="cmb_SelectedIndexChanged" OnTextChanged="grid_TextChanged" runat="server" Text='<%# String.Format("{0:N2}", Decimal.Parse(Eval("Pago")), 2) %>' CssClass="estandar-control pago" autocomplete="off" placeholder="0.00" ID="txt_pago" style="text-align:right"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Descuentos">--%>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Deducible">
                                                <ItemTemplate>
                                                    <asp:TextBox AutoPostBack="True" OnTextChanged="grid_TextChanged" runat="server" Text='<%# String.Format("{0:N2}", Decimal.Parse(Eval("Descuentos")), 2) %>' CssClass="estandar-control Descuentos" autocomplete="off" placeholder="0.00" style="text-align:right"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="RFC">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="True" runat="server" Text='<%# Eval("RFC") %>' CssClass="estandar-control"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--  <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Deducibles">
                                                <ItemTemplate>
                                                    <asp:TextBox AutoPostBack="True" OnTextChanged="grid_TextChanged" runat="server" Text='<%# Eval("Deducible") %>' CssClass="estandar-control deducibles" autocomplete="off"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Tipo de pago">
                                                <ItemTemplate>
                                                    <asp:DropDownList AutoPostBack="true" runat="server" ClientIDMode="Static" CssClass="estandar-control tipo_pago" OnSelectedIndexChanged="cmb_SelectedIndexChanged">
                                                        <asp:ListItem Value="P">PARCIAL</asp:ListItem>
                                                        <asp:ListItem Value="F">FINAL</asp:ListItem>
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--FJCP MULTIPAGO AGREGUE ESTE TEMPLATE PARA AGREGAR UNA NUEVA COLUMNA DE FOLIO ONBASE--%>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Folio OnBase">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("FolioOnbase") %>' CssClass="estandar-control factura"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--FJCP NUM PAGO AGREGUE ESTE TEMPLATE PARA AGREGAR UNA NUEVA COLUMNA DE NUMERO DE PAGO--%>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Numero Pago">
                                                <ItemTemplate>
                                                    <asp:TextBox ReadOnly="true" runat="server" Text='<%# Eval("NumeroPago") %>' CssClass="estandar-control "></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <%--importes de la poliza --%>

                    <div class="row">
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Importes en Moneda de la Póliza</asp:Label>
                        </div>
                        <div class="form-group col-md-2">
                            <%--En este caso es el subtotal para terceros y asegurados --%>
                            <asp:Label runat="server" class="etiqueta-control">Importe Autorización</asp:Label>
                            <asp:TextBox ReadOnly="true" ID="txtTotalAutorizacion" runat="server" Text='<%# Eval("TotalAutorizacion") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Total de IVA</asp:Label>
                            <asp:TextBox ReadOnly="true" ID="txtTotalImpuestos" runat="server" Text='<%# Eval("TotalImpuestos") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Total Retenciones</asp:Label>
                            <asp:TextBox ReadOnly="true" ID="txtTotalRetenciones" runat="server" Text='<%# Eval("TotalRetenciones") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Total</asp:Label>
                            <asp:TextBox ReadOnly="true" ID="txtTotal" runat="server" Text='<%# Eval("Total") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                            <asp:TextBox Visible="false" ID="txtTotalNacional" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                    </div>

                    <%--importe del pago--%>
                    <div class="row">
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Importes en Moneda del Pago</asp:Label>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ReadOnly="true" ID="iptxtTotalAutorizacion" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ReadOnly="true" ID="iptxtTotalImpuestos" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ReadOnly="true" ID="iptxtTotalRetenciones" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ReadOnly="true" ID="iptxtTotal" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                            <asp:TextBox Visible="false" ID="iptxtTotalNacional" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                        </div>
                    </div>

                    <%--importes de la Factura--%>
                    <div id="Facturas0" runat="server">
                        <div class="row">
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control">Importes de las Facturas Relacionadas</asp:Label>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control"> </asp:Label>
                                <asp:TextBox ReadOnly="true" ID="txtTotalAutorizacionFac" runat="server" Text='<%# Eval("TotalAutorizacionFac") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control"></asp:Label>
                                <asp:TextBox ReadOnly="true" ID="txtTotalImpuestosFac" runat="server" Text='<%# Eval("TotalImpuestosFac") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control"></asp:Label>
                                <asp:TextBox ReadOnly="true" ID="txtTotalRetencionesFac" runat="server" Text='<%# Eval("TotalRetencionesFac") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control"></asp:Label>
                                <asp:TextBox ReadOnly="true" ID="txtTotalFac" runat="server" Text='<%# Eval("TotalFac") %>' CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                                <asp:TextBox Visible="false" ID="txtTotalNacionalFac" runat="server" CssClass="estandar-control Tablero" placeholder="0.00" style="text-align:right"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div id="Facturas1" runat="server">
                        <div class="row">
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control"></asp:Label>
                            </div>
                            <%--FJCP MEJORAS Importe en pago a Proveedores. Se pone propiedad visible en false ini--%>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control" Visible="false">Descuentos</asp:Label>
                                <asp:TextBox ReadOnly="true" ID="txtDescuentos" runat="server" Text='<%# Eval("TotalAutorizacionNacionalFac") %>' CssClass="estandar-control Tablero" placeholder="0.00" Visible="false"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label runat="server" class="etiqueta-control" Visible="false">SubTotal</asp:Label>
                                <asp:TextBox ReadOnly="true" ID="txtTotalAutorizacionNacionalFac" runat="server" Text='<%# Eval("TotalRetenciones") %>' CssClass="estandar-control Tablero" placeholder="0.00" Visible="false"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-2">
                                <asp:Label ID="lbldescuento" runat="server" ForeColor="Red" class="etiqueta-control" Visible="false"></asp:Label>
                            </div>
                            <%--FJCP MEJORAS Importe en pago a Proveedores. Se pone propiedad visible en false fin--%>
                        </div>
                    </div>
                </div>
                <div class="cuadro-titulo panel-encabezado">
                    <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana2" class="contraer" />
                    <input type="image" src="../Images/expander_mini_inv.png" id="exVentana2" class="expandir" />
                    Orden de pago
                </div>
                <asp:HiddenField runat="server" ID="oBancoT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oSucursalT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oBeneficiarioT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oMonedaT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oTipoCuentaT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oCuentaBancariaT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oPlazaT_stro" Value="" />
                <asp:HiddenField runat="server" ID="oAbaT_stro" Value="" />
                <asp:HiddenField runat="server" ID="hidCodUsuario" Value="" />
                <asp:HiddenField runat="server" ID="FondoSinIva" Value="" />
                <asp:HiddenField runat="server" ID="hidBloqueoOnbase" Value="1" />    <%--FJCP_10290_CC--%>
                <div class="panel-contenido ventana2">
                    <div class="row">
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Sucursal</asp:Label>
                            <asp:DropDownList readonly="true" ID="cmbSucursal" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                                <asp:ListItem Value="1">CIUDAD DE MEXICO</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group col-md-4">
                            <asp:Label runat="server" class="etiqueta-control">Beneficiario</asp:Label>
                            <asp:TextBox ID="txtBeneficiario" runat="server" CssClass="estandar-control Tablero Centro" ReadOnly="true" placeholder="Beneficiario"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" class="etiqueta-control">Concepto de orden de pago</asp:Label>
                            <asp:TextBox ID="txtConceptoOP" runat="server" Text='<%# Eval("ConceptoOP") %>' CssClass="estandar-control Tablero" ReadOnly="true" placeholder="Concepto de orden de pago"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Origen</asp:Label>
                            <asp:DropDownList ID="cmbOrigenOP" AutoPostBack="true" runat="server" ClientIDMode="Static" ReadOnly="true" CssClass="estandar-control Tablero"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Label runat="server" class="etiqueta-control">Tipo de pago</asp:Label>
                            <asp:DropDownList AutoPostBack="true" ID="cmbTipoPagoOP" runat="server" ClientIDMode="Static" CssClass="estandar-control tipo_pago_OP Tablero" OnSelectedIndexChanged="cmb_SelectedIndexChanged">
                                <asp:ListItem Value="C">CHEQUE</asp:ListItem>
                                <asp:ListItem Value="T">TRANSFERENCIA</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group col-md-2">
                            <div style="width: 100%; text-align: left; margin-top: 14px;">
                                <asp:LinkButton ID="btnVerCuentas" runat="server" class="btn btn-primary btn-xs" Style="background-color: #003A5D;">
                                    <span>
                                        <img class="btn-añadir"/> VER CUENTAS
                                    </span>
                                </asp:LinkButton>
                            </div>
                        </div>
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" class="etiqueta-control">Concepto 2</asp:Label>
                            <asp:TextBox ID="txtcpto2" runat="server" CssClass="estandar-control Tablero" placeholder="Concepto"></asp:TextBox>
                        </div>
                    </div>
                    <%--FJCP PAGO TESOFE--%>
                    <div class="row">
                        <div class="form-group col-md-4">
                            <asp:Label ID="lblDependencias" runat="server" class="etiqueta-control" Visible="false">Dependencias</asp:Label>
                            <asp:DropDownList AutoPostBack="true" ID="drDependencias" runat="server" CssClass="estandar-control Tablero" Visible="false">
                                <%--                                <asp:ListItem Value="C">CHEQUE</asp:ListItem>
                                <asp:ListItem Value="T">TRANSFERENCIA</asp:ListItem>--%>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <%--FJCP TESOFE FIN--%>

                    <!--JLC Mejoras Analista Solicitante -Inicio -->
                    <div class="row">
                        <div class="form-group col-md-4">

                            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label runat="server" class="etiqueta-control">Analista Solicitante</asp:Label>
                                    <asp:DropDownList ID="cmbAnalistaSolicitante" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                                    </asp:DropDownList>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cmbOrigenOP" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>


                        </div>
                    </div>
                    <!--JLC Mejoras Analista Solicitante -Fin -->

                    <div style="width: 100%; text-align: left">
                        <asp:LinkButton ID="btnGrabarOP" runat="server" class="btn btn-primary btn-sm" Style="background-color: #006AA9;">
                                    <span>
                                        <img class="btn-guardar"/> GENERAR OP
                                    </span>
                        </asp:LinkButton>
                    </div>
                    <div style="width: 100%; text-align: right">
                        <asp:LinkButton ID="btnLimpiar" runat="server" class="btn btn-primary btn-sm" Style="background-color: #006AA9;">
                                    <span>
                                        <img class="btn-limpiar"/> LIMPIAR
                                    </span>
                        </asp:LinkButton>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <br />
    </div>
    <!--JLC Mejoras Tipo de Cambio Pactado -Inicio -->
    <div id="Modal" class="modal-catalogo">
        <div class="cuadro-titulo-flotante">
            <button type="button" data-dismiss="modal" class="close">&times;</button>
            <div>
                <label id="lbl_Catalogo">Consultar Tipo de Cambio</label>
            </div>
        </div>
        <div class="clear padding5"></div>

        <div class="input-group">
            <br />
            <div class="row">
                <div class="form-group col-md-1"></div>
                <div class="form-group col-md-5">
                    <asp:Label ID="Label2" runat="server" class="etiqueta-control">Fecha a Consultar</asp:Label>
                </div>
                <div class="form-group col-md-5">
                    <asp:TextBox ID="txt_fecha_ini" runat="server" CssClass=" estandar-control fechadepago  Fecha Centro" autocomplete="off" placeholder="DD/MM/YYYY" AutoPostBack="true"></asp:TextBox>
                </div>
                <div class="form-group col-md-1"></div>
            </div>
            <div class="row">
                <div class="form-group col-md-2"></div>
                <div class="form-group col-md-8">
                    <asp:UpdatePanel runat="server" ID="upCatalogo" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="txt_tipoCambioConsultado" runat="server" CssClass="estandar-control  Centro" placeholder="1.00"></asp:TextBox>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="txt_fecha_ini" EventName="TextChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div class="form-group col-md-2"></div>
            </div>
            <br />
            <br />
        </div>
        <div style="width: 100%; text-align: right;">
            <button class="btn botones" id="btn_Aceptar" type="button" data-dismiss="modal">Aceptar</button>
            <asp:Button runat="server" ID="btn_Cancela_OP" class="btn botones" Text="Cancelar" data-dismiss="modal" />
        </div>
    </div>
    <!--JLC Mejoras Tipo de Cambio Pactado -Fin -->
</asp:Content>

