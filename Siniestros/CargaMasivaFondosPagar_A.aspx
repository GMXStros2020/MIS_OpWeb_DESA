<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="CargaMasivaFondosPagar_A.aspx.vb" Inherits="Siniestros_OrdenPago" %>
<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="1|0" />
    <script src="../Scripts/Siniestros/OrdenPagoFondos.js"></script>
    <style>
        .table>tbody>tr>td{
            padding: 0px;
        }

        .table>tbody>tr>td>input, .table>tbody>tr>td>select{
            border: 0px;
        }

        .chkeliminar{
            border: 0px;
        }

        .table>tbody>tr>th{
            padding: 2px;
        }

        .zona-principal{
            padding-right: 10px;
        }

        .btnEjemplo{
            margin-top: 20px;
        }

        @media (max-width:1080px){
            
            .zona-fechas{
               display:none;
            }

            .zona-form{
                padding-left:20px;
            }
        }

    </style>
    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <asp:UpdatePanel runat="server" ID="upGenerales">
            <ContentTemplate>
                <div class="cuadro-titulo panel-encabezado">
                    <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana1" class="contraer" />
                    <input type="image" src="../Images/expander_mini_inv.png" id="exVentana1" class="expandir" />
                     Fondos / Carga Masiva Pagar A <asp:Label ID="lblTitulo" runat="server" Text=""></asp:Label>
                </div>
                <asp:HiddenField runat="server" ID="hid_Operacion" Value="0" />
                <div id="div_GVD" style="width:100%;height:350px;overflow-x:scroll;">
                    <asp:GridView runat="server" ID="gvd_CargaMasivaIngPrimas" AutoGenerateColumns="false" GridLines="Horizontal" 
                        CssClass="grid-view" HeaderStyle-CssClass="header" AlternatingRowStyle-CssClass="altern"
                        ShowHeaderWhenEmpty="false" DataKeyNames="" >
                        <Columns>

                            <asp:TemplateField HeaderText="" HeaderStyle-Width="2%" ItemStyle-CssClass="SeleccionCampo" HeaderStyle-CssClass="Centrado">
                                <ItemTemplate>
                                    <asp:textbox runat="server" Enabled="false" ID="lbl_Folio_Onbase" CssClass="Centrado etiqueta-subseccion-grid" Text='<%# Eval("Folio_Onbase")%>' Width="100%"></asp:textbox>
                                    <asp:textbox runat="server"  ID="_lbl_Folio_Onbase" CssClass="NoDisplay" Text='<%# Eval("Folio_Onbase")%>'></asp:textbox>
                                </ItemTemplate>
                            </asp:TemplateField>   

                            <asp:TemplateField HeaderText="" HeaderStyle-Width="2%" ItemStyle-CssClass="SeleccionCampo" HeaderStyle-CssClass="Centrado">
                                <ItemTemplate>
                                    <asp:textbox runat="server" Enabled="false" ID="lbl_SucReasPrim" CssClass="Centrado etiqueta-subseccion-grid" Text='<%# Eval("Pagar_A")%>' Width="100%"></asp:textbox>
                                    <asp:textbox runat="server"  ID="_lbl_SucReasPrim" CssClass="NoDisplay" Text='<%# Eval("Pagar_A")%>'></asp:textbox>
                                </ItemTemplate>
                            </asp:TemplateField>                        

                        </Columns>
                    </asp:GridView>
                </div>
                <div style="width:100%; text-align:right; border-top-style:inset; border-width:1px; border-color:#003A5D"> 
                    <asp:LinkButton id="btn_CargarLayout" runat="server" Width="130px" class="btn botones">
                            <span>
                                <img class="btn-cargar"/>
                                Cargar layout
                            </span>
                        </asp:LinkButton>                                                       
                    <asp:LinkButton id="btn_ValidarReas" runat="server" class="btn botones">
                            <span>
                                <img class="btn-buscar"/>
                                Validar
                            </span>
                        </asp:LinkButton>                        
                    <asp:LinkButton id="btn_AgregaReasPrim" runat="server" Visible="false" class="btn botones">
                        <span>
                            <img class="btn-añadir"/>
                            Agregar
                        </span>
                    </asp:LinkButton>                    
                        
                    <asp:LinkButton id="btn_LimpiarReasPrim" runat="server" class="btn botones">
                        <span>
                            <img class="btn-limpiar"/>
                            Limpiar
                        </span>
                    </asp:LinkButton>
                    <asp:LinkButton id="btn_CerrarReasPrim" runat="server" class="btn botones">
                        <span>
                            <img class="btn-cancelar"/>
                            Cerrar
                        </span>
                    </asp:LinkButton>                   
                </div>
                <div id="CargaLayoutIngresosPrimas" style="width:40%" class="modal-catalogo" >
        <asp:UpdatePanel runat="server" ID="UpdatePanel14">
            <ContentTemplate>
                <div class="cuadro-titulo">
                    <button type="button" class="close"  data-dismiss="modal">&times;</button>
                    Importar ingresos de primas
                </div>
                <div class="cuadro-subtitulo-grid" style="width:80%;text-align:left;">
                    <asp:label runat="server" ID="Label3" CssClass="Centro" Width="100%"></asp:label>                    
                </div>
                <asp:HiddenField runat="server" ID="HiddenField1" Value="0" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="clear padding5"></div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel15">
             <ContentTemplate>
                 <asp:FileUpload ID="fiu_ReasPrim" CssClass="estandar-control" Width="100%" runat="server" />                    
             </ContentTemplate>
             <Triggers>
                 <asp:PostBackTrigger ControlID="btn_CargaLayoutIngPrimas" />
             </Triggers>
        </asp:UpdatePanel>
        <div class="clear padding5"></div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel16">
            <ContentTemplate>
                <div style="width:100%; text-align:right;border-top-style:inset; border-width:1px; border-color:#003A5D">
                    <asp:LinkButton id="btn_CargaLayoutIngPrimas" runat="server" class="btn botones">
                        <span>
                            <img class="btn-cargar"/>
                            Cargar
                        </span>
                    </asp:LinkButton>
                    <asp:LinkButton id="btn_CancelarCargaLayout" runat="server" data-dismiss="modal" class="btn botones">
                        <span>
                            <img class="btn-cancelar"/>
                            Cancelar
                        </span>
                    </asp:LinkButton>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>    
            </ContentTemplate>
        </asp:UpdatePanel>
        <br /><br />
    </div>

</asp:Content>
