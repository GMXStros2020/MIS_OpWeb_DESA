jQuery(document).ready(function () {


    var outerwidth = $("#list47").width();
    $("#txt_width").val(outerwidth)




    var res = { '0': 'Sin Informacion' };
    var Tipo_Pago = { '0': 'Seleccione Opcion', '1': 'CHEQUE', '2': 'TRANSFERENCIA' };






    $("#btn_Reporte").click(function () {


        $("#btn_Guardar").addClass("hidden");
        $("[id*=btn_Enviar]").addClass("hidden");
        $("[id*=btn_Revisar]").addClass("hidden");
        $("[id*=btn_guardar_xls]").addClass("hidden");
        $("[id*=btn_exportar_xls]").addClass("hidden");
        $("[id*=txt_NumLote]").val("");


        var txt_fecha_ini = $("[id*=txt_fecha_ini]").val();

        var txt_fecha_fin = $("[id*=txt_fecha_fin]").val();



        var Folio_OnBase = $("[id*=txt_folio_onbase_Desde]").val();

        var Folio_OnBase_hasta = $("[id*=txt_folio_onbase_Hasta]").val();

        var PagarA = $("[id*=cmbPagarA]").val();
        var TipoPago = $("[id*=cmbTipoPago]").val();
        var TipoComprobante = $("[id*=cmbTipoComprobante]").val();
        var MonedaPago = $("[id*=cmbMonedaPago]").val();
        var RFC = $("[id*=txtRFC]").val();
        var SubSiniestro = $("[id*=cmbSubsiniestro]").val();
        var TipoPago_PoT = $("[id*=cmbTipoPagoPoT]").val();
        var Siniestro = $("[id*=txtSiniestro]").val();
        var sn_multipago;

        if ($('[id*=chkMultipago]').is(':checked')) {
            sn_multipago = -1;
        }
        else {
            sn_multipago = 0;
        }


        RFC = RFC.replace("&", "!");

        var cod_analista = $("[id*=cmbAnalistaSolicitante]").val();
        var VariasFacturas = $("[id*=chkVariasFacturas]").val();
        var FondosSinIVA = $("[id*=chkFondosSinIVA]").val();

        if (VariasFacturas == 'on') {
            VariasFacturas = "Y"
        }
        else {
            VariasFacturas = "N"
        }


        if (FondosSinIVA == 'on') {
            FondosSinIVA = "Y"
        }
        else {
            FondosSinIVA = "N"
        }


        if (txt_fecha_fin == "") {
            fn_MuestraMensaje("Atencion", "Favor de capturar el rango Fecha de aceptación del documento", 0);
            return;
        }


        if (txt_fecha_fin == "") {
            fn_MuestraMensaje("Atencion", "Favor de capturar el rango Fecha de aceptación del documento", 0);
            return;
        }


        $("#loading").removeClass("hidden");
        $("#list47").jqGrid("clearGridData");
        $("#list47").jqGrid("GridUnload")





        $.ajax({
            url: "../Siniestros/OrdenPagoMasivo.ashx?fecha_ini=" + txt_fecha_ini + "&fecha_fin=" + txt_fecha_fin + "&PagarA=" + PagarA + "&Folio_OnBase=" + Folio_OnBase + "&Folio_OnBase_hasta=" + Folio_OnBase_hasta + "&TipoPago=" + TipoPago + "&TipoComprobante=" + TipoComprobante + "&MonedaPago=" + MonedaPago + "&RFC=" + RFC + "&SubSiniestro=" + SubSiniestro + "&VariasFacturas" + VariasFacturas + "&cod_analista=" + cod_analista + "&FondosSinIVA=" + FondosSinIVA + "&TipoPago_PoT=" + TipoPago_PoT + "&Siniestro=" + Siniestro + "&sn_multipago=" + sn_multipago,
            type: "POST",
            success: function (result) {



                var mydata = $.parseJSON(result);
                if (PagarA == 8) {
                    LoadGridTerceros(mydata);
                }
                else if (PagarA == 7) {
                    LoadGridAsegurados(mydata);
                }
                else {
                    LoadGrid(mydata);
                }




            },
            error: function (err) {



            }
        });












    });


    $("#btn_Guardar").click(function () {

        var myGrid = $('#list47');
        var myArray = [];
        var myIDs = myGrid.jqGrid('getDataIDs');
        $("#loading").removeClass("hidden");


        var txt_NumLote = $("[id*=txt_NumLote]").val();
        var Fec_pago = $("[id*=txtFechaEstimadaPago]").val();
        ////var selectedRowId = myGrid.jqGrid("getGridParam", 'selrow');
        ////var selectedRowData = myGrid.getRowData(selectedRowId);
        ////var selectedRowIds = myGrid.jqGrid("getGridParam", 'selarrrow');
        ////var selectedRowData;




        ////for (selectedRowIndex = 0; selectedRowIndex < selectedRowIds.length; selectedRowIndex++) {

        ////    selectedRowData = myGrid.getRowData(selectedRowIds[selectedRowIndex]);
        ////    selectedRowData.Folio_Onbase = selectedRowData.FolioOnbaseHidden
        ////    if (selectedRowData.Concepto_Pago.indexOf('input') > 0)
        ////    {
        ////        selectedRowData.Concepto_Pago = "";
        ////    }

        ////    if (selectedRowData.Concepto2.indexOf('input') > 0) {
        ////        selectedRowData.Concepto2 = "";
        ////    }

        ////    if (selectedRowData.Cuenta_Bancaria.indexOf('input') > 0) {
        ////        selectedRowData.Cuenta_Bancaria = "";
        ////    }

        ////    if (selectedRowData.Notas.indexOf('input') > 0) {
        ////        selectedRowData.Notas = "";
        ////    }

        ////    if (selectedRowData.Observaciones.indexOf('input') > 0) {
        ////        selectedRowData.Observaciones = "";
        ////    }

        ////    if (selectedRowData.Confirmar_Cuenta.indexOf('input') > 0) {
        ////        selectedRowData.Confirmar_Cuenta = "";
        ////    }

        ////    myArray[selectedRowIndex] = selectedRowData;
        ////}


        for (var i = 0; i < myIDs.length; i++) {

            // nos traemos renglon a renglon           

            var myRow = myGrid.jqGrid('getRowData', myIDs[i]);
            myRow.Folio_Onbase = myRow.FolioOnbaseHidden;
            myRow.Folio_Onbase_cuenta = myRow.Folio_Onbase_cuentaHidden;
            myRow.Fec_pago = Fec_pago;
            if (myRow.Concepto_Pago.indexOf('input') > 0) {
                myRow.Concepto_Pago = "";
            }

            if (myRow.Concepto2.indexOf('input') > 0) {
                myRow.Concepto2 = "";
            }

            if (myRow.Cuenta_Bancaria.indexOf('input') > 0) {
                myRow.Cuenta_Bancaria = "";
            }

            if (myRow.Notas.indexOf('input') > 0) {
                myRow.Notas = "";
            }

            if (myRow.Observaciones.indexOf('input') > 0) {
                myRow.Observaciones = "";
            }

            if (myRow.Confirmar_Cuenta.indexOf('input') > 0) {
                myRow.Confirmar_Cuenta = "";
            }


            myArray[i] = myRow;

        }

        var json = JSON.stringify(myArray)
        if (txt_NumLote == "") {
            txt_NumLote = "0";
        }




        $.ajax({
            url: "../LocalServices/OrdenPagoMasiva.asmx/SetOP",
            data: "{ 'myArray': " + json + ",'Lote':" + txt_NumLote + "}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                fn_MuestraMensaje('Atencion', 'Numero de Lote Generado ' + data.d, 0, "");
                $("[id*=txt_NumLote]").val(data.d);
                $("#loading").addClass("hidden");
                $("#loading2").addClass("hidden");
                $("[id*=btn_Revisar]").removeClass("hidden");
                $("[id*=btn_Enviar]").removeClass("hidden");

            },
            error: function (response) {

                $("#loading").addClass("hidden");

                fn_MuestraMensaje('Error', response.responseText, 0, "")

            },
        });





    });

    $("#btn_guardar_xls").click(function () { // Boton para guardar en una tabla temporal los registros para subsiniestros varios y multipago 
        var myGrid = $('#list47');
        var myArray = [];
        var myIDs = myGrid.jqGrid('getDataIDs');
        var Fec_pago = $("[id*=txtFechaEstimadaPago]").val();
        $("#loading").removeClass("hidden");

        for (var i = 0; i < myIDs.length; i++) {
            // nos traemos renglon a renglon           
            var myRow = myGrid.jqGrid('getRowData', myIDs[i]);
            myRow.Folio_Onbase = myRow.FolioOnbaseHidden;
            myRow.Folio_Onbase_cuenta = myRow.Folio_Onbase_cuentaHidden;
            myRow.Fec_pago = Fec_pago;

            if (myRow.Concepto_Pago.indexOf('input') > 0) {
                myRow.Concepto_Pago = "";
            }

            if (myRow.Concepto2.indexOf('input') > 0) {
                myRow.Concepto2 = "";
            }

            if (myRow.Cuenta_Bancaria.indexOf('input') > 0) {
                myRow.Cuenta_Bancaria = "";
            }

            if (myRow.Notas.indexOf('input') > 0) {
                myRow.Notas = "";
            }

            if (myRow.Observaciones.indexOf('input') > 0) {
                myRow.Observaciones = "";
            }

            if (myRow.Confirmar_Cuenta.indexOf('input') > 0) {
                myRow.Confirmar_Cuenta = "";
            }

            myArray[i] = myRow;
        }

        var json = JSON.stringify(myArray);

        txt_NumLote = -1;

        $.ajax({ //Ajax para grabar temporal para siniestros varios y multipago 
            url: "../LocalServices/OrdenPagoMasiva.asmx/Export_xls",
            data: "{ 'myArray': " + json + ",'Lote':" + txt_NumLote + "}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                //fn_MuestraMensaje('Atencion', 'Numero de Lote Generado ' + data.d, 0, "");
                //$("[id*=txt_NumLote]").val(data.d);
                $("#loading").addClass("hidden");
                $("#loading2").addClass("hidden");
                $("[id*=btn_exportar_xls]").removeClass("hidden");
                $("[id*=btn_guardar_xls]").addClass("hidden");
                $("[id*=hid_nLote]").val(data.d);
            },
            error: function (response) {
                $("#loading").addClass("hidden");
                fn_MuestraMensaje('Error', response.responseText, 0, "");
            }
        });
    });
    //////////////////////////////////////////////////////////////////////////////


    $("#btn_Recuperar_lote").click(function () {
        
        var txt_NumLote = $("[id*=txt_NumLote]").val();
        $("#btn_Guardar").addClass("hidden");
        $("[id*=btn_Enviar]").addClass("hidden");
        $("[id*=btn_Revisar]").addClass("hidden");
        var PagarA = $("[id*=cmbPagarA]").val();
        
        if (txt_NumLote == "") {
            fn_MuestraMensaje("Atencion", "Favor de capturar el Numero de lote a recuperar", 0);
            return;
        }        

        $("#loading").removeClass("hidden");
        $("#list47").jqGrid("clearGridData");
        $("#list47").jqGrid("GridUnload")
        var lastsel2;

        $.ajax({
            url: "../LocalServices/OrdenPagoMasiva.asmx/RecuperaLote",
            data: "{ 'NumLote': " + txt_NumLote + "}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (result) {

                var mydata = $.parseJSON(result.d);
                $("[id*=txtFechaEstimadaPago]").val(mydata[0].Fec_pago);

                
                if (mydata[0].PagarA == "Tercero") {
                    PagarA = 8;
                }

                if (mydata[0].PagarA == "Asegurado") {
                    PagarA = 7;
                }

                //FJCP_10290_CC INI
                if (mydata[0].PagarA == "Proveedor") {
                    PagarA = 10;
                }

                $("[id*=cmbPagarA]").val(PagarA);
                //FJCP_10290_CC FIN


                if (PagarA == 8) {
                    LoadGridTerceros(mydata);
                }
                else if (PagarA == 7) {
                    LoadGridAsegurados(mydata);
                }
                else {
                    LoadGrid(mydata);
                }

                $("[id*=btn_Enviar]").removeClass("hidden");
                $("[id*=btn_Revisar]").removeClass("hidden");


            },
            error: function (err) {
                debugger
                alert(err);

            }
        });








    });

    function CargarConceptoPago(TipoUsuario, ClasePago, CodigoPres) {

        $.ajax({
            url: "../Siniestros/Catalogos.ashx?Catalgo=ConceptoPago",
            data: "{ 'TipoUsuario': " + TipoUsuario + ",'ClasePago':" + ClasePago + " ,'CodigoPres':" + CodigoPres + "}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (result) {


                var s = '<option value="0">Seleccione Opcion</option>';

                for (var i = 0; i < result.length; i++) {

                    s += '<option value="' + result[i].Concepto + '">' + result[i].Descripcion + '</option>';

                }


                res = s;
                return s;

            },
            error: function (err) {
                debugger
                alert(err);

            }
        });
    };
    


    function LoadGrid(mydata) {
        var lastsel2;
        var lastSel = -1;

        
        jQuery("#list47").jqGrid({

            data: mydata,
            datatype: "local",
            height: 280,
            width: $("#txt_width").val(),
            rowNum: 8000,
            rowList: [10, 20, 30],
            colNames: ['Folio Onbase', 'Num Pago', 'Tipo de comprobante', 'Pagar A', 'Codigo', 'RFC', 'Nombre /Razon Social', 'Siniestro', 'Subsinientro', 'Moneda', 'Tipo de Cambio', 'Reserva', 'Moneda de Pago', 'Importe', 'Descuentos', 'Importe del concepto', 'Concepto Facturado', 'cod_concepto_pago', 'Concepto de pago', 'cod_clas_Pago', 'Clase de Pago', 'cod_tipo_pago', 'Tipo de Pago', 'Concepto 2', 'Tipo de Pago', 'Folio Onbase Estado de cuenta', 'Cuenta Bancaria', 'Confirmar Cuenta', 'Solicitante', 'Notas', 'Observaciones', 'id_tipo_Doc', 'moneda', 'moneda pago', 'FolioOnbaseHidden', 'Folio_Onbase_cuentaHidden', 'Id_persona', '', '', , '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', 'Poliza', 'Fec_pago', 'Accion'],
            colModel: [

                { name: 'Folio_Onbase', index: 'Folio_Onbase', width: 100, frozen: false },
                { name: 'Num_Pago', index: 'Num_Pago', width: 100 },
                { name: 'Tipo_comprobante', index: 'Tipo_comprobante', width: 182 },
                { name: 'PagarA', index: 'PagarA', width: 90 },
                { name: 'CodigoCliente', index: 'CodigoCliente', width: 90 },
                { name: 'RFC', index: 'RFCc', width: 120 },
                { name: 'Nombre_Razon_Social', index: 'Nombre_Razon_Social', width: 300 },
                { name: 'Siniestro', index: 'Siniestro', width: 90 },
                { name: 'Subsiniestro', index: 'Subsiniestro', width: 90 },
                { name: 'Moneda', index: 'Moneda', width: 180 },
                { name: 'Tipo_Cambio', index: 'Tipo_Cambio', width: 90, formatter: "number.toFixed(4)", align: 'right', sorttype: "int" },
                { name: 'Reserva', index: 'Reserva', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                { name: 'Moneda_Pago', index: 'Moneda_Pago', width: 180 },
                { name: 'Importe', index: 'Importe', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                { name: 'Deducible', index: 'Deducible', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                { name: 'Importe_concepto', index: 'Importe_concepto', width: 90, formatter: "number", align: 'right', align: 'right', sorttype: "int" },
                { name: 'Concepto_Factura', index: 'Concepto_Factura', width: 90 },
                { name: 'Cod_concepto_pago', index: 'Cod_concepto_pago', width: 45, hidden: true },
                {
                    name: 'Concepto_Pago', index: 'Concepto_Pago', align: 'center', width: 270, editable: true, edittype: "select", editoptions: {
                        value: res,
                        dataEvents: [
                            {
                                type: 'change',
                                fn: function (e) {
                                    var v = parseInt($(e.target).val(), 10);
                                    var row = $(e.target).closest('tr.jqgrow');
                                    var rowId = row.attr('id');
                                    jQuery("#list47").jqGrid('setCell', rowId, 'Cod_concepto_pago', v);
                                    RecuperarClasePago(rowId, v, -1)
                                }
                            }
                        ]

                    }
                },
                //{
                //    name: 'Concepto_Pago', index: 'Concepto_Pago', align: 'center', width: 270, editable: true, edittype: "select",
                //    editoptions: {                        
                //        aysnc: false, dataUrl: '../Siniestros/Catalogos.ashx?Catalgo=CptoPagoFondosProv',
                //        buildSelect: function (data) {
                //            var response = jQuery.parseJSON(data);
                //            var s = '<select>';
                //            s += '<option value="0">Seleccione Opcion</option>';
                //            $($.parseJSON(data)).map(function () {
                //                s += '<option value="' + this.Concepto + '">' + this.Descripcion + '</option>';
                //            });
                //            return s + "</select>";
                //        },
                //        dataEvents: [
                //            {
                //                type: 'change',
                //                fn: function (e) {
                //                    var v = parseInt($(e.target).val(), 10);
                //                    var com = jQuery("#list47").jqGrid('getRowData', rowId).Folio_Onbase;
                //                    var row = $(e.target).closest('tr.jqgrow');
                //                    var rowId = row.attr('id');
                //                    var FolioOnbase = jQuery("#list47").jqGrid('getRowData', rowId).FolioOnbaseHidden;
                //                    var ClasePago = v;

                //                    $.ajax({
                //                        url: "../Siniestros/Catalogos.ashx?Catalgo=ClasePagoFondos&FolioOnbase=" + FolioOnbase,
                //                        dataType: "json",
                //                        type: "POST",
                //                        contentType: "application/json; charset=utf-8",
                //                        success: function (result) {
                //                            var s = '<option value="0">Seleccione valor</option>';
                //                            for (var i = 0; i < result.length; i++) {
                //                                s += '<option value="' + result[i].cod_clase_pago + '">' + result[i].txt_desc + '</option>';
                //                            }
                //                            res = s;
                //                            $("select#" + rowId + "_Clase_pago", row[0]).html(res);
                //                            jQuery("#list47").jqGrid('setCell', rowId, 'Cod_concepto_pago', v);
                //                            RecuperarClasePago(rowId, v, -1)
                //                        },

                //                        error: function (err) {
                //                            debugger
                //                            alert(err);
                //                        }
                //                    });
                //                }
                //            }
                //        ]

                //    }
                //},
                { name: 'Cod_clas_pago', index: 'Cod_clas_pago', width: 45, hidden: true },
                //{   name: 'Clase_pago',
                //    hidden: false,
                //    //sortable: true,
                //    index: 'Clase_pago',
                //    width: 155,
                //    align: 'center',
                //    editable: true,
                //    edittype: "select",
                //    editoptions: {
                //        value: res,
                //        dataEvents: [
                //            {
                //                type: 'change',
                //                fn: function (e) {
                //                    var v = parseInt($(e.target).val(), 10);
                //                    var row = $(e.target).closest('tr.jqgrow');
                //                    var rowId = row.attr('id');
                //                    jQuery("#list47").jqGrid('setCell', rowId, 'Cod_clas_pago', v);
                //                }
                //            }
                //        ]
                //    },
                //},
                { name: 'Clase_pago', index: 'Clase_pago', width: 180, hidden: false },

                { name: 'Cod_tipo_pago', index: 'Cod_tipo_pago', width: 45, hidden: true },
                { name: 'Tipo_Pago', index: 'Tipo_Pago', width: 180 },
                { name: 'Concepto2', index: 'Concepto2', width: 180, editable: true, hidden: false, editoptions: { size: "30", maxlength: "100" } },
                { name: 'Tipo_Pago2', index: 'Tipo_Pago2', width: 90, hidden: false },
                { name: 'Folio_Onbase_cuenta', index: 'Folio_Onbase_cuenta', width: 90, hidden: true },
                { name: 'Cuenta_Bancaria', index: 'Cuenta_Bancaria', width: 180, editable: false, editoptions: { size: "30", maxlength: "18" }, editrules: { custom: true, custom_func: Validar, required: true } },
                { name: 'Confirmar_Cuenta', index: 'Confirmar_Cuenta', width: 180, editable: false, editoptions: { size: "30", maxlength: "18" }, editrules: { custom: true, custom_func: Validar, required: true } },
                { name: 'Solicitante', index: 'Solicitante', width: 90 },
                { name: 'Notas', index: 'Notas', width: 180, editable: true, editoptions: { size: "30", maxlength: "100" } },
                { name: 'Observaciones', index: 'Observaciones', width: 260, editable: true, editoptions: { size: "50", maxlength: "100" } },
                { name: 'Id_Tipo_Doc', index: 'Id_Tipo_Doc', width: 90, hidden: true },
                { name: 'Cod_moneda', index: 'Cod_moneda', width: 90, hidden: true },
                { name: 'Cod_moneda_pago', index: 'Cod_moneda_pago', width: 90, hidden: true },
                { name: 'FolioOnbaseHidden', index: 'FolioOnbaseHidden', width: 90, hidden: true },
                { name: 'Folio_Onbase_cuentaHidden', index: 'Folio_Onbase_cuentaHidden', width: 90, hidden: true },
                { name: 'Id_Persona', index: 'Id_Persona', width: 90, hidden: true },

                { name: 'CodigoSucursal', index: 'CodigoSucursal', width: 90, hidden: true },
                { name: 'TipoMovimiento', index: 'TipoMovimiento', width: 90, hidden: true },
                { name: 'VariasFacturas', index: 'VariasFacturas', width: 90, hidden: true },
                { name: 'Ramo', index: 'Ramo', width: 90, hidden: true },
                { name: 'SubRamo', index: 'SubRamo', width: 90, hidden: true },
                { name: 'ID_TipoComprobante', index: 'ID_TipoComprobante', width: 90, hidden: true },
                { name: 'NumeroComprobante', index: 'NumeroComprobante', width: 90, hidden: true },
                { name: 'FechaComprobante', index: 'FechaComprobante', width: 90, hidden: true },
                { name: 'CodTipoStro', index: 'CodTipoStro', width: 90, hidden: true },
                { name: 'CodigoOrigenPago', index: 'CodigoOrigenPago', width: 90, hidden: true },
                { name: 'FechaIngreso', index: 'FechaIngreso', width: 90, hidden: true },
                { name: 'CodigoBancoTransferencia', index: 'CodigoBancoTransferencia', width: 90, hidden: true },

                { name: 'IdSiniestro', index: 'IdSiniestro', width: 90, hidden: true },
                { name: 'CodigoTercero', index: 'CodigoTercero', width: 90, hidden: true },
                { name: 'Subtotal', index: 'Subtotal', width: 90, hidden: true },
                { name: 'Iva', index: 'Iva', width: 90, hidden: true },
                { name: 'Total', index: 'Total', width: 90, hidden: true },
                { name: 'Retencion', index: 'Retencion', width: 90, hidden: true },
                { name: 'CodItem', index: 'CodItem', width: 90, hidden: true },
                { name: 'CodIndCob', index: 'CodIndCob', width: 90, hidden: true },
                { name: 'NumeroCorrelaEstim', index: 'NumeroCorrelaEstim', width: 90, hidden: true },
                { name: 'NumeroCorrelaPagos', index: 'NumeroCorrelaPagos', width: 90, hidden: true },
                { name: 'SnCondusef', index: 'SnCondusef', width: 90, hidden: true },
                { name: 'NumeroOficioCondusef', index: 'NumeroOficioCondusef', width: 90, hidden: true },
                { name: 'TipoPagoDetalle', index: 'TipoPagoDetalle', width: 90, hidden: true },
                { name: 'Cod_objeto', index: 'Cod_objeto', width: 90, hidden: true },
                { name: 'Poliza', index: 'Poliza', width: 90, hidden: true },
                { name: 'Fec_pago', index: 'Fec_pago', width: 180, hidden: true },

                {
                    name: 'myac', width: 60, fixed: true, sortable: false, resize: false, formatter: 'actions',
                    formatoptions: { keys: true }, frozen: false
                },


            ],
            onSelectRow: function (id) {
                if (id && id !== lastsel2) {
                    jQuery('#list47').jqGrid('restoreRow', lastsel2);
                    // jQuery('#list47').jqGrid('editRow', id, true);
                    lastsel2 = id;
                }
            },
            ondblClickRow: function (id, ri, ci) {
                if (id && id !== lastSel) {
                    jQuery("#list47").restoreRow(lastSel);
                    lastSel = id;
                }

                Celdas(id);


                //var TipoUsuario = jQuery("#list47").jqGrid('getRowData', id).PagarA;
                //var TipoPago = jQuery("#list47").jqGrid('getRowData', id).Tipo_Pago2

                //if (TipoUsuario == "Proveedor") {
                //    jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                //    jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

                //}
                //else {
                //    if (TipoPago == "TRANSFERENCIA") {
                //        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: true });
                //        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: true });
                //    }
                //    else {
                //        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                //        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
                //    }

                //    jQuery("#list47").setColProp('Importe', { editable: true });
                //    jQuery("#list47").setColProp('Importe_concepto', { editable: true });


                //}



                jQuery("#list47").editRow(id, true, null, null, '../LocalServices/OrdenPagoMasiva.asmx/Apoyo', null,
                    function (rowid, response) {  // aftersavefunc
                        //  grid.setColProp('State', { editoptions: { value: states } });

                        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

                    });
                return;
            },


            pager: "#page_columnas",
            viewrecords: true,
            caption: "Listado de Folios",
            autowidth: false,
            shrinkToFit: false,
            forceFit: true,
            loadonce: true,
            multiselect: false,
            rownumbers: true,
            editurl: '../LocalServices/OrdenPagoMasiva.asmx/Apoyo',

                     

            ajaxSelectOptions: {
                error: function (xhr, status, error) {

                    alert(error);
                }
            },
            // height: 'auto'
        });


        //FJCP_10290_CC INI
        
        jQuery("#list47").jqGrid('navGrid', '#page_columnas', { add: false, edit: false, del: false, search: false, refresh: false });
        //jQuery("#list47").jqGrid('navGrid', '#page_columnas', { add: false, edit: false, del: false, search: false, refresh: false, position: "left", cloneToTop: true });
        
        jQuery("#list47").jqGrid('navButtonAdd', '#page_columnas', {
            caption: "Ocultar/Mostrar Columnas",
            title: "Reorder Columns",
            //position: "last",
            onClickButton: function () {
                jQuery("#list47").jqGrid('columnChooserFondosProv');
            }
        });
        //FJCP_10290_CC FIN

        jQuery("#list47").jqGrid('setFrozenColumns')
        
        
        $("#loading").addClass("hidden");     

      
            var siniestro = $("[id*=txtSiniestro]").val();            
            var sRFC = $("[id*=txtRFC]").val();

            if (siniestro == undefined || siniestro == null) {
                siniestro = "";
            }

            if (sRFC == undefined || sRFC == null) {
                sRFC = "";
            }

            sRFC = sRFC.replace("&", "!");
                    
            if (siniestro != "" && sRFC != "") {
                $("[id*=btn_guardar_xls]").removeClass("hidden");
            }
            else {
                if ($('[id*=chkMultipago]').is(':checked')) {
                    $("[id*=btn_guardar_xls]").removeClass("hidden");
                }
                else {
                    $("#btn_Guardar").removeClass("hidden");
                }                   
            }               
            
        
    };



    function LoadGridAsegurados(mydata) {
        var lastsel2;
        var lastSel = -1;

        jQuery("#list47").jqGrid({

            data: mydata,
            datatype: "local",
            height: 280,
            width: $("#txt_width").val(),
            rowNum: 8000,
            rowList: [10, 20, 30],
            colNames: ['Folio Onbase', 'Num Pago', 'Tipo de comprobante', 'Pagar A', 'Codigo', 'RFC', 'Nombre /Razon Social','AltaTercero', 'Siniestro', 'Subsinientro', 'Moneda', 'Tipo de Cambio', 'Reserva', 'Moneda de Pago', 'Importe', 'Deducible', 'Importe del concepto', 'Concepto Facturado', 'cod_concepto_pago', 'Concepto de pago', 'cod_clas_Pago', 'Clase de Pago', 'cod_tipo_pago', 'Tipo de Pago', 'Concepto 2', 'Tipo de Pago', 'Folio Onbase Estado de cuenta', 'Cuenta Bancaria', 'Cuenta_Bancaria_ok', 'Confirmar Cuenta', 'Confirmar Cuenta_ok', 'Solicitante', 'Notas', 'Observaciones', 'id_tipo_Doc', 'moneda', 'moneda pago', 'FolioOnbaseHidden', 'Folio_Onbase_cuentaHidden', 'Id_persona', '', '', , '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', 'Poliza', 'Fec_pago', 'Accion'],
            colModel: [

                { name: 'Folio_Onbase', index: 'Folio_Onbase', width: 100, frozen: false },
                { name: 'Num_Pago', index: 'Num_Pago', width: 100 },
                { name: 'Tipo_comprobante', index: 'Tipo_comprobante', width: 182 },
                { name: 'PagarA', index: 'PagarA', width: 90 },
                { name: 'CodigoCliente', index: 'CodigoCliente', width: 90, editable: true },
                { name: 'RFC', index: 'RFCc', width: 120, editable: true },
                { name: 'Nombre_Razon_Social', index: 'Nombre_Razon_Social', width: 300, editable: true  },
                { name: 'AltaTercero', index: 'AltaTercero', width: 100 },
                { name: 'Siniestro', index: 'Siniestro', width: 90 },
                { name: 'Subsiniestro', index: 'Subsiniestro', width: 90 },
                { name: 'Moneda', index: 'Moneda', width: 180 },
                { name: 'Tipo_Cambio', index: 'Tipo_Cambio', width: 90, formatter: "number.toFixed(4)", align: 'right', sorttype: "int" },
                { name: 'Reserva', index: 'Reserva', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                {name: 'Moneda_Pago', index: 'Moneda_Pago', width: 180, editable: true, edittype: "select", editoptions: {
                        value: "0:NACIONAL;1:DOLAR AMERICANO"
                    }
                },
                { name: 'Importe', index: 'Importe', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                { name: 'Deducible', index: 'Deducible', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                { name: 'Importe_concepto', index: 'Importe_concepto', width: 90, formatter: "number", align: 'right', sorttype: "int", hidden: true},
                { name: 'Concepto_Factura', index: 'Concepto_Factura', width: 90 },

                { name: 'Cod_concepto_pago', index: 'Cod_concepto_pago', width: 45, hidden: true },
                {
                    name: 'Concepto_Pago', index: 'Concepto_Pago', align: 'center', width: 270, editable: true, edittype: "select",
                    editoptions: {
                        value: res,
                        dataEvents: [
                            {
                                type: 'change',
                                fn: function (e) {
                                    var v = parseInt($(e.target).val(), 10);
                                    var row = $(e.target).closest('tr.jqgrow');
                                    var rowId = row.attr('id');
                                    jQuery("#list47").jqGrid('setCell', rowId, 'Cod_concepto_pago', v);



                                    RecuperarClasePago(rowId, v, 0)
                                }
                            }
                        ]

                    }
                },
                //{
                //    name: 'Concepto_Pago', index: 'Concepto_Pago', align: 'center', width: 270, editable: true, edittype: "select", editoptions: {


                //        aysnc: false, dataUrl: '../Siniestros/Catalogos.ashx?Catalgo=CptoPagoFondosAse',
                        
                //        buildSelect: function (data) {

                //            var response = jQuery.parseJSON(data);

                //            var s = '<select>';
                //            s += '<option value="0">Seleccione Opcion</option>';
                //            $($.parseJSON(data)).map(function () {


                //                s += '<option value="' + this.Concepto + '">' + this.Descripcion + '</option>';
                //            });

                //            return s + "</select>";
                //        },
                //        dataEvents: [
                //            {
                //                type: 'change',
                //                fn: function (e) {
                //                    var v = parseInt($(e.target).val(), 10);
                //                    var com = jQuery("#list47").jqGrid('getRowData', rowId).Folio_Onbase;




                //                    var row = $(e.target).closest('tr.jqgrow');
                //                    var rowId = row.attr('id');

                //                    var FolioOnbase = jQuery("#list47").jqGrid('getRowData', rowId).FolioOnbaseHidden;
                //                    var ClasePago = v;



                //                    $.ajax({
                //                        url: "../Siniestros/Catalogos.ashx?Catalgo=ClasePagoFondos&FolioOnbase=" + FolioOnbase,
                //                        dataType: "json",
                //                        type: "POST",
                //                        contentType: "application/json; charset=utf-8",
                //                        success: function (result) {


                //                            var s = '<option value="0">Seleccione valor</option>';

                //                            for (var i = 0; i < result.length; i++) {

                //                                s += '<option value="' + result[i].cod_clase_pago + '">' + result[i].txt_desc + '</option>';

                //                            }


                //                            res = s;
                //                            $("select#" + rowId + "_Clase_pago", row[0]).html(res);



                //                            jQuery("#list47").jqGrid('setCell', rowId, 'Cod_concepto_pago', v);
                //                            RecuperarClasePago(rowId, v, 0)

                //                        },
                //                        error: function (err) {
                //                            debugger
                //                            alert(err);

                //                        }
                //                    });
                //                }
                //            }
                //        ]

                //    }
                //}
                //,



                { name: 'Cod_clas_pago', index: 'Cod_clas_pago', width: 45, hidden: true },
                //{
                //    name: 'Clase_pago',
                //    hidden: false,
                //    //sortable: true,,

                //    index: 'Clase_pago',
                //    width: 155,

                //    align: 'center',

                //    editable: true,
                //    edittype: "select",
                //    editoptions: {
                //        value: res,

                //        dataEvents: [
                //            {
                //                type: 'change',
                //                fn: function (e) {
                //                    var v = parseInt($(e.target).val(), 10);





                //                    var row = $(e.target).closest('tr.jqgrow');
                //                    var rowId = row.attr('id');





                //                    jQuery("#list47").jqGrid('setCell', rowId, 'Cod_clas_pago', v);






                //                }
                //            }
                //        ]
                //    },







                //},
                { name: 'Clase_pago', index: 'Clase_pago', width: 180, hidden: false },
                { name: 'Cod_tipo_pago', index: 'Cod_tipo_pago', width: 45, hidden: true }

                ,
                { name: 'Tipo_Pago', index: 'Tipo_Pago', width: 180 },
                { name: 'Concepto2', index: 'Concepto2', width: 180, editable: true, hidden: false, editoptions: { size: "30", maxlength: "100" } },
                { name: 'Tipo_Pago2', index: 'Tipo_Pago2', width: 90, hidden: false },
                { name: 'Folio_Onbase_cuenta', index: 'Folio_Onbase_cuenta', width: 90 },

                { name: 'Cuenta_Bancaria', index: 'Cuenta_Bancaria', width: 180, formatter: MascaraPwdCta, editable: false, editoptions: { size: "30", maxlength: "18" }, editrules: { custom: true, custom_func: Validar, required: true } },
                { name: 'Cuenta_Bancaria_ok', index: 'Cuenta_Bancaria_ok', width: 180, hidden: true }, //FJCP_10290_CC
                { name: 'Confirmar_Cuenta', index: 'Confirmar_Cuenta', width: 180, formatter: MascaraPwdCta, editable: false, editoptions: { size: "30", maxlength: "18" }, editrules: { custom: true, custom_func: Validar, required: true } },
                { name: 'Confirmar_Cuenta_ok', index: 'Confirmar_Cuenta_ok', width: 180, hidden: true }, //FJCP_10290_CC

                { name: 'Solicitante', index: 'Solicitante', width: 90 },
                { name: 'Notas', index: 'Notas', width: 180, editable: true, editoptions: { size: "30", maxlength: "100" } },
                { name: 'Observaciones', index: 'Observaciones', width: 260, editable: true, editoptions: { size: "50", maxlength: "100" } },
                { name: 'Id_Tipo_Doc', index: 'Id_Tipo_Doc', width: 90, hidden: true },
                { name: 'Cod_moneda', index: 'Cod_moneda', width: 90, hidden: true },
                { name: 'Cod_moneda_pago', index: 'Cod_moneda_pago', width: 90, hidden: true },
                { name: 'FolioOnbaseHidden', index: 'FolioOnbaseHidden', width: 90, hidden: true },
                { name: 'Folio_Onbase_cuentaHidden', index: 'Folio_Onbase_cuentaHidden', width: 90, hidden: true },
                { name: 'Id_Persona', index: 'Id_Persona', width: 90, hidden: true },

                { name: 'CodigoSucursal', index: 'CodigoSucursal', width: 90, hidden: true },
                { name: 'TipoMovimiento', index: 'TipoMovimiento', width: 90, hidden: true },
                { name: 'VariasFacturas', index: 'VariasFacturas', width: 90, hidden: true },
                { name: 'Ramo', index: 'Ramo', width: 90, hidden: true },
                { name: 'SubRamo', index: 'SubRamo', width: 90, hidden: true },
                { name: 'ID_TipoComprobante', index: 'ID_TipoComprobante', width: 90, hidden: true },
                { name: 'NumeroComprobante', index: 'NumeroComprobante', width: 90, hidden: true },
                { name: 'FechaComprobante', index: 'FechaComprobante', width: 90, hidden: true },
                { name: 'CodTipoStro', index: 'CodTipoStro', width: 90, hidden: true },
                { name: 'CodigoOrigenPago', index: 'CodigoOrigenPago', width: 90, hidden: true },
                { name: 'FechaIngreso', index: 'FechaIngreso', width: 90, hidden: true },
                { name: 'CodigoBancoTransferencia', index: 'CodigoBancoTransferencia', width: 90, hidden: true },

                { name: 'IdSiniestro', index: 'IdSiniestro', width: 90, hidden: true },
                { name: 'CodigoTercero', index: 'CodigoTercero', width: 90, hidden: true },
                { name: 'Subtotal', index: 'Subtotal', width: 90, hidden: true },
                { name: 'Iva', index: 'Iva', width: 90, hidden: true },
                { name: 'Total', index: 'Total', width: 90, hidden: true },
                { name: 'Retencion', index: 'Retencion', width: 90, hidden: true },
                { name: 'CodItem', index: 'CodItem', width: 90, hidden: true },
                { name: 'CodIndCob', index: 'CodIndCob', width: 90, hidden: true },
                { name: 'NumeroCorrelaEstim', index: 'NumeroCorrelaEstim', width: 90, hidden: true },
                { name: 'NumeroCorrelaPagos', index: 'NumeroCorrelaPagos', width: 90, hidden: true },
                { name: 'SnCondusef', index: 'SnCondusef', width: 90, hidden: true },
                { name: 'NumeroOficioCondusef', index: 'NumeroOficioCondusef', width: 90, hidden: true },
                { name: 'TipoPagoDetalle', index: 'TipoPagoDetalle', width: 90, hidden: true },
                { name: 'Cod_objeto', index: 'Cod_objeto', width: 90, hidden: true },
                { name: 'Poliza', index: 'Poliza', width: 90, hidden: true },
                { name: 'Fec_pago', index: 'Fec_pago', width: 180, hidden: true },









                {
                    name: 'myac', width: 60, fixed: true, sortable: false, resize: false, formatter: 'actions',
                    formatoptions: { keys: true }, frozen: false
                },


            ],
            onSelectRow: function (id) {
                if (id && id !== lastsel2) {
                    jQuery('#list47').jqGrid('restoreRow', lastsel2);
                    // jQuery('#list47').jqGrid('editRow', id, true);
                    lastsel2 = id;
                }
            },
            ondblClickRow: function (id, ri, ci) {
                if (id && id !== lastSel) {
                    jQuery("#list47").restoreRow(lastSel);
                    lastSel = id;
                }

                Celdas(id);

                //var TipoUsuario = jQuery("#list47").jqGrid('getRowData', id).PagarA;
                //var TipoPago = jQuery("#list47").jqGrid('getRowData', id).Tipo_Pago2

                //if (TipoUsuario == "Proveedor") {
                //    jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                //    jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

                //}
                //else {
                //    if (TipoPago == "TRANSFERENCIA") {
                //        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: true, edittype: "password" }); //FJCP_10290_CC
                //        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: true, edittype: "password" }); //FJCP_10290_CC
                //    }
                //    else {
                //        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                //        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
                //    }

                //    jQuery("#list47").setColProp('Importe', { editable: true });
                //    jQuery("#list47").setColProp('Importe_concepto', { editable: true });


                //}


                jQuery("#list47").editRow(id, true, null, null, '../LocalServices/OrdenPagoMasiva.asmx/Apoyo', null,
                    function (rowid, response) {  // aftersavefunc
                        //  grid.setColProp('State', { editoptions: { value: states } });

                        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

                    });
                return;
            },


            pager: "#page_columnas",
            viewrecords: true,
            caption: "Listado de Folios",
            autowidth: false,
            shrinkToFit: false,
            forceFit: true,
            loadonce: true,
            multiselect: false,
            rownumbers: true,
            editurl: '../LocalServices/OrdenPagoMasiva.asmx/Apoyo',

            ajaxSelectOptions: {
                error: function (xhr, status, error) {

                    alert(error);
                }
            },
            // height: 'auto'
        });

        //FJCP_10290_CC INI
        
            jQuery("#list47").jqGrid('navGrid', '#page_columnas', { add: false, edit: false, del: false, search: false, refresh: false });
        //jQuery("#list47").jqGrid('navGrid', '#page_columnas', { add: false, edit: false, del: false, search: false, refresh: false, position: "left", cloneToTop: true });

        jQuery("#list47").jqGrid('navButtonAdd', '#page_columnas', {
            caption: "Ocultar/Mostrar Columnas",
            title: "Reorder Columns",
            //position: "last",
            onClickButton: function () {
                jQuery("#list47").jqGrid('columnChooserFondosAseg');
            }
        });
        //FJCP_10290_CC FIN


        jQuery("#list47").jqGrid('setFrozenColumns')






        $("#loading").addClass("hidden");
        var siniestro = $("[id*=txtSiniestro]").val();
        var sRFC = $("[id*=txtRFC]").val();

        if (siniestro == undefined || siniestro == null) {
            siniestro = "";
        }

        if (sRFC == undefined || sRFC == null) {
            sRFC = "";
        }

        sRFC = sRFC.replace("&", "!");

        if (siniestro != "" && sRFC != "") {
            $("[id*=btn_guardar_xls]").removeClass("hidden");
        }
        else {
            if ($('[id*=chkMultipago]').is(':checked')) {
                $("[id*=btn_guardar_xls]").removeClass("hidden");
            }
            else {
                $("#btn_Guardar").removeClass("hidden");
            }
        }  





    };



    function MascaraPwdCta(cellvalue, options, rowObject, columnName) { //FJCP_10290_CC
        var id = options.rowId;
        var pwdEncript = "******************";

        if (cellvalue == undefined || cellvalue == null || cellvalue == "") {
            return "";
        }
        else {
            return pwdEncript;
        }
    }


    function LoadGridTerceros(mydata) {
        var lastsel2;
        var lastSel = -1;

        try {
            jQuery("#list47").jqGrid({

                data: mydata,
                datatype: "local",

                height: 280,
                width: $("#txt_width").val(),
                rowNum: 8000,
                rowList: [10, 20, 30],
                colNames: ['Folio Onbase', 'Num Pago', 'Tipo de comprobante', 'Pagar A', 'Codigo', 'RFC', 'Nombre /Razon Social', 'Alta Tercero', 'Siniestro', 'Subsinientro', 'Moneda', 'Tipo de Cambio', 'Reserva', 'Moneda de Pago', 'Importe', 'Deducible', 'Importe del concepto', 'Concepto Facturado', 'cod_clas_Pago', 'Clase de Pago', 'cod_concepto_pago', 'Concepto de pago', 'cod_tipo_pago', 'Tipo de Pago', 'Concepto 2', 'Tipo de Pago', 'Folio Onbase Estado de cuenta', 'Cuenta Bancaria', 'Cuenta_Bancaria_ok', 'Confirmar Cuenta', 'Confirmar_Cuenta_ok', 'Solicitante', 'Notas', 'Observaciones', 'id_tipo_Doc', 'moneda', 'moneda pago', 'FolioOnbaseHidden', 'Folio_Onbase_cuentaHidden', 'Id_persona', '', '', , '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', 'Poliza', '', 'Accion'],
                colModel: [

                    { name: 'Folio_Onbase', index: 'Folio_Onbase', width: 100, frozen: false },
                    { name: 'Num_Pago', index: 'Num_Pago', width: 100 },
                    { name: 'Tipo_comprobante', index: 'Tipo_comprobante', width: 182 },
                    { name: 'PagarA', index: 'PagarA', width: 90 },
                    { name: 'CodigoCliente', index: 'CodigoCliente', width: 90, editable: true },
                    { name: 'RFC', index: 'RFCc', width: 120, editable: true },
                    { name: 'Nombre_Razon_Social', index: 'Nombre_Razon_Social', width: 300, editable: true  },
                    { name: 'AltaTercero', index: 'AltaTercero', width: 100 },
                    { name: 'Siniestro', index: 'Siniestro', width: 90 },
                    { name: 'Subsiniestro', index: 'Subsiniestro', width: 90 },
                    { name: 'Moneda', index: 'Moneda', width: 180 },
                    { name: 'Tipo_Cambio', index: 'Tipo_Cambio', width: 90, formatter: "number.toFixed(4)", align: 'right', sorttype: "int" },
                    { name: 'Reserva', index: 'Reserva', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                    { name: 'Moneda_Pago', index: 'Moneda_Pago', width: 180, editable: true, edittype: "select", editoptions: {
                            value: "0:NACIONAL;1:DOLAR AMERICANO"
                        }},
                    { name: 'Importe', index: 'Importe', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                    { name: 'Deducible', index: 'Deducible', width: 90, formatter: "number", align: 'right', sorttype: "int" },
                    { name: 'Importe_concepto', index: 'Importe_concepto', width: 90, formatter: "number", align: 'right', sorttype: "int", hidden: true },
                    { name: 'Concepto_Factura', index: 'Concepto_Factura', width: 90 },
                    { name: 'Cod_clas_pago', index: 'Cod_clas_pago', width: 180, hidden: true },
                    { name: 'Clase_pago', index: 'Clase_pago', width: 180, hidden: false },
                    { name: 'Cod_concepto_pago', index: 'Cod_concepto_pago', width: 180, hidden: true },
                    {
                        name: 'Concepto_Pago', index: 'Concepto_Pago', align: 'center', width: 270, editable: true, edittype: "select", editoptions: {
                            value: res,
                            dataEvents: [
                                {
                                    type: 'change',
                                    fn: function (e) {
                                        var v = parseInt($(e.target).val(), 10);

                                        var row = $(e.target).closest('tr.jqgrow');
                                        var rowId = row.attr('id');

                                        jQuery("#list47").jqGrid('setCell', rowId, 'Cod_concepto_pago', v);
                                        RecuperarClasePago(rowId, v, 0)
                                    }
                                }
                            ]

                        }
                    }
                    ,
                    { name: 'Cod_tipo_pago', index: 'Cod_tipo_pago', width: 45, hidden: true }

                    ,
                    { name: 'Tipo_Pago', index: 'Tipo_Pago', width: 180 }
                    ,
                    { name: 'Concepto2', index: 'Concepto2', width: 180, editable: true, hidden: false, editoptions: { size: "30", maxlength: "100" } },
                    { name: 'Tipo_Pago2', index: 'Tipo_Pago2', width: 90, hidden: false },
                    { name: 'Folio_Onbase_cuenta', index: 'Folio_Onbase_cuenta', width: 90 },

                    { name: 'Cuenta_Bancaria', index: 'Cuenta_Bancaria', width: 180, formatter: MascaraPwdCta,  editable: false, editoptions: { size: "30", maxlength: "18" }, editrules: { custom: true, custom_func: Validar, required: true } },
                    { name: 'Cuenta_Bancaria_ok', index: 'Cuenta_Bancaria_ok', width: 180, hidden: true }, //FJCP_10290_CC
                    { name: 'Confirmar_Cuenta', index: 'Confirmar_Cuenta', width: 180, formatter: MascaraPwdCta,  editable: false, editoptions: { size: "30", maxlength: "18" }, editrules: { custom: true, custom_func: Validar, required: true } },
                    { name: 'Confirmar_Cuenta_ok', index: 'Confirmar_Cuenta_ok', width: 180, hidden: true }, //FJCP_10290_CC



                    { name: 'Solicitante', index: 'Solicitante', width: 90 },
                    { name: 'Notas', index: 'Notas', width: 180, editable: true, editoptions: { size: "30", maxlength: "100" } },
                    { name: 'Observaciones', index: 'Observaciones', width: 260, editable: true, editoptions: { size: "50", maxlength: "100" } },
                    { name: 'Id_Tipo_Doc', index: 'Id_Tipo_Doc', width: 90, hidden: true },
                    { name: 'Cod_moneda', index: 'Cod_moneda', width: 90, hidden: true },
                    { name: 'Cod_moneda_pago', index: 'Cod_moneda_pago', width: 90, hidden: true },
                    { name: 'FolioOnbaseHidden', index: 'FolioOnbaseHidden', width: 90, hidden: true },
                    { name: 'Folio_Onbase_cuentaHidden', index: 'Folio_Onbase_cuentaHidden', width: 90, hidden: true },
                    { name: 'Id_Persona', index: 'Id_Persona', width: 90, hidden: true },

                    { name: 'CodigoSucursal', index: 'CodigoSucursal', width: 90, hidden: true },
                    { name: 'TipoMovimiento', index: 'TipoMovimiento', width: 90, hidden: true },
                    { name: 'VariasFacturas', index: 'VariasFacturas', width: 90, hidden: true },
                    { name: 'Ramo', index: 'Ramo', width: 90, hidden: true },
                    { name: 'SubRamo', index: 'SubRamo', width: 90, hidden: true },
                    { name: 'ID_TipoComprobante', index: 'ID_TipoComprobante', width: 90, hidden: true },
                    { name: 'NumeroComprobante', index: 'NumeroComprobante', width: 90, hidden: true },
                    { name: 'FechaComprobante', index: 'FechaComprobante', width: 90, hidden: true },
                    { name: 'CodTipoStro', index: 'CodTipoStro', width: 90, hidden: true },
                    { name: 'CodigoOrigenPago', index: 'CodigoOrigenPago', width: 90, hidden: true },
                    { name: 'FechaIngreso', index: 'FechaIngreso', width: 90, hidden: true },
                    { name: 'CodigoBancoTransferencia', index: 'CodigoBancoTransferencia', width: 90, hidden: true },

                    { name: 'IdSiniestro', index: 'IdSiniestro', width: 90, hidden: true },
                    { name: 'CodigoTercero', index: 'CodigoTercero', width: 90, hidden: true },
                    { name: 'Subtotal', index: 'Subtotal', width: 90, hidden: true },
                    { name: 'Iva', index: 'Iva', width: 90, hidden: true },
                    { name: 'Total', index: 'Total', width: 90, hidden: true },
                    { name: 'Retencion', index: 'Retencion', width: 90, hidden: true },
                    { name: 'CodItem', index: 'CodItem', width: 90, hidden: true },
                    { name: 'CodIndCob', index: 'CodIndCob', width: 90, hidden: true },
                    { name: 'NumeroCorrelaEstim', index: 'NumeroCorrelaEstim', width: 90, hidden: true },
                    { name: 'NumeroCorrelaPagos', index: 'NumeroCorrelaPagos', width: 90, hidden: true },
                    { name: 'SnCondusef', index: 'SnCondusef', width: 90, hidden: true },
                    { name: 'NumeroOficioCondusef', index: 'NumeroOficioCondusef', width: 90, hidden: true },
                    { name: 'TipoPagoDetalle', index: 'TipoPagoDetalle', width: 90, hidden: true },
                    { name: 'Cod_objeto', index: 'Cod_objeto', width: 90, hidden: true },
                    { name: 'Poliza', index: 'Poliza', width: 90, hidden: true },
                    { name: 'Fec_pago', index: 'Fec_pago', width: 180, hidden: true },


                    {
                        name: 'myac', width: 60, fixed: true, sortable: false, resize: false, formatter: 'actions',
                        formatoptions: { keys: true }, frozen: false
                    },


                ],
                onSelectRow: function (id) {
                    if (id && id !== lastsel2) {
                        jQuery('#list47').jqGrid('restoreRow', lastsel2);
                        // jQuery('#list47').jqGrid('editRow', id, true);
                        lastsel2 = id;
                    }
                },
                ondblClickRow: function (id, ri, ci) {
                    if (id && id !== lastSel) {
                        jQuery("#list47").restoreRow(lastSel);
                        lastSel = id;
                    }

                    Celdas(id);

                    //var TipoUsuario = jQuery("#list47").jqGrid('getRowData', id).PagarA;
                    //var TipoPago = jQuery("#list47").jqGrid('getRowData', id).Tipo_Pago2
                    //if (TipoUsuario == "Proveedor") {
                    //    jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                    //    jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

                    //}
                    //else {
                    //    if (TipoPago == "TRANSFERENCIA") {                            
                    //        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: true, edittype: "password" }); //FJCP_10290_CC
                    //        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: true, edittype: "password" }); //FJCP_10290_CC
                    //    }
                    //    else {
                    //        jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                    //        jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
                    //    }

                    //    jQuery("#list47").setColProp('Importe', { editable: true });
                    //    jQuery("#list47").setColProp('Importe_concepto', { editable: true });


                    //}

                    jQuery("#list47").editRow(id, true, null, null, '../LocalServices/OrdenPagoMasiva.asmx/Apoyo', null,
                        function (rowid, response) {  // aftersavefunc
                            //  grid.setColProp('State', { editoptions: { value: states } });

                            jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                            jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

                        });
                    return;
                },


                pager: "#page_columnas",
                viewrecords: true,
                caption: "Listado de Folios",
                autowidth: false,
                shrinkToFit: false,
                forceFit: true,
                loadonce: true,
                multiselect: false,
                rownumbers: true,
                editurl: '../LocalServices/OrdenPagoMasiva.asmx/Apoyo',

                ajaxSelectOptions: {
                    error: function (xhr, status, error) {

                        alert(error);
                    }
                },
                // height: 'auto'
            });


            //FJCP_10290_CC INI
            
                jQuery("#list47").jqGrid('navGrid', '#page_columnas', { add: false, edit: false, del: false, search: false, refresh: false });
            //jQuery("#list47").jqGrid('navGrid', '#page_columnas', { add: false, edit: false, del: false, search: false, refresh: false, position: "left", cloneToTop: true });

            jQuery("#list47").jqGrid('navButtonAdd', '#page_columnas', {
                caption: "Ocultar/Mostrar Columnas",
                title: "Reorder Columns",
                //position: "last",
                onClickButton: function () {
                    jQuery("#list47").jqGrid('columnChooserFondosTer');
                }
            });
        //FJCP_10290_CC FIN


            jQuery("#list47").jqGrid('setFrozenColumns')






            $("#loading").addClass("hidden");
            var siniestro = $("[id*=txtSiniestro]").val();
            var sRFC = $("[id*=txtRFC]").val();

            if (siniestro == undefined || siniestro == null) {
                siniestro = "";
            }

            if (sRFC == undefined || sRFC == null) {
                sRFC = "";
            }

            sRFC = sRFC.replace("&", "!");

            if (siniestro != "" && sRFC != "") {
                $("[id*=btn_guardar_xls]").removeClass("hidden");
            }
            else {
                if ($('[id*=chkMultipago]').is(':checked')) {
                    $("[id*=btn_guardar_xls]").removeClass("hidden");
                }
                else {
                    $("#btn_Guardar").removeClass("hidden");
                }
            }  


        } catch (error) {
            console.error(error);
            // expected output: ReferenceError: nonExistentFunction is not defined
            // Note - error messages will vary depending on browser
        }


    };


    function Celdas(id) {
        var TipoUsuario = jQuery("#list47").jqGrid('getRowData', id).PagarA;
        var CodigoPres = jQuery("#list47").jqGrid('getRowData', id).CodigoCliente;
        var id_tipo_pago = jQuery("#list47").jqGrid('getRowData', id).TipoPagoDetalle
        var poliza = jQuery("#list47").jqGrid('getRowData', id).Poliza;
        var Concepto_Pago = jQuery("#list47").jqGrid('getRowData', id).Concepto_Pago
        var Nombre_Razon_Social = jQuery("#list47").jqGrid('getRowData', id).Nombre_Razon_Social
        var TipoPago = jQuery("#list47").jqGrid('getRowData', id).Tipo_Pago2
        var cta_clabe = jQuery("#list47").jqGrid('getRowData', id).Cuenta_Bancaria
        var codAnalista = $("[id*=cmbAnalistaSolicitante]").val();





        if (CodigoPres == "") {
            CodigoPres = 0
        }
        if (TipoUsuario == "Proveedor") {
            jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
            jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });

        }
        else {

            if (TipoPago == "TRANSFERENCIA") {
                if (cta_clabe != "") {
                    jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                    jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
                }
                else {
                    jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: true, edittype: "password" });
                    jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: true, edittype: "password" });
                }
            }
            else {
                jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
            }

            jQuery("#list47").setColProp('Importe', { editable: true });
            jQuery("#list47").setColProp('Importe_concepto', { editable: true });

        }



        var row = $(id.target).closest('tr.jqgrow');

        $.ajax({            
            url: "../Siniestros/Catalogos.ashx?Catalgo=ConceptoPagoFondos&TipoUsuario=" + TipoUsuario + "&id_tipo_pago=" + id_tipo_pago + "&CodigoPres=" + CodigoPres + "&codAnalista=" + codAnalista,
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (result) {


                //var s = '<option value="0">Seleccione valor</option>';
                var vhid_cpto_pago = $("#hid_cpto_pago").val();
                var vhid_cpto_pago_desc = $("#hid_cpto_pago_desc").val();
                var s = '';
                for (var i = 0; i < result.length; i++) {


                    if (vhid_cpto_pago == "") {
                        if (Concepto_Pago == result[i].Descripcion) {
                            s += '<option value="' + result[i].Concepto + '"  selected = "selected">' + result[i].Descripcion + '</option>';
                        }
                        else {
                            s += '<option value="' + result[i].Concepto + '">' + result[i].Descripcion + '</option>';
                        }
                    }
                    else {
                        if (vhid_cpto_pago_desc == result[i].Descripcion) {
                            s += '<option value="' + result[i].Concepto + '"  selected = "selected">' + result[i].Descripcion + '</option>';
                        }
                        else {
                            s += '<option value="' + result[i].Concepto + '">' + result[i].Descripcion + '</option>';
                        }
                    }
                }
                res = s;
                $("select#" + id + "_Concepto_Pago", row[0]).html(res);

                $("#hid_cpto_pago").val("");
                $("#hid_cpto_pago_desc").val("");
            },
            error: function (err) {
                debugger
                alert(err);
            }
        });
        
        if (TipoUsuario == "Proveedor") {
             jQuery("#list47").setColProp('Nombre_Razon_Social', { editable: false });
        }
        else {
            jQuery("#list47").setColProp('Nombre_Razon_Social', { editable: true });
        }


     
            var siniestro = $("[id*=txtSiniestro]").val();
            var sRFC = $("[id*=txtRFC]").val();

            if (siniestro == undefined || siniestro == null) {
                siniestro = "";
            }

            if (sRFC == undefined || sRFC == null) {
                sRFC = "";
            }

            sRFC = sRFC.replace("&", "!");

            if (siniestro != "" && sRFC != "") {
                jQuery("#list47").setColProp('Concepto_Pago', { editable: false });
                jQuery("#list47").setColProp('Concepto2', { editable: false });
                jQuery("#list47").setColProp('Notas', { editable: false });
                jQuery("#list47").setColProp('Observaciones', { editable: false });
                jQuery("#list47").setColProp('Nombre_Razon_Social', { editable: false });
                jQuery("#list47").setColProp('Moneda_Pago', { editable: false });
                jQuery("#list47").setColProp('Importe', { editable: false });
                jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
            }

            if ($('[id*=chkMultipago]').is(':checked')) {
                jQuery("#list47").setColProp('Concepto_Pago', { editable: false });
                jQuery("#list47").setColProp('Concepto2', { editable: false });
                jQuery("#list47").setColProp('Notas', { editable: false });
                jQuery("#list47").setColProp('Observaciones', { editable: false });
                jQuery("#list47").setColProp('Nombre_Razon_Social', { editable: false });
                jQuery("#list47").setColProp('Moneda_Pago', { editable: false });
                jQuery("#list47").setColProp('Importe', { editable: false });
                jQuery("#list47").setColProp('Cuenta_Bancaria', { editable: false });
                jQuery("#list47").setColProp('Confirmar_Cuenta', { editable: false });
            }
        

    };

    function Validar(valor, columnName, length) {

        var savedRow = jQuery("#list47").getGridParam("savedRow");

        var id = jQuery("#list47").jqGrid('getGridParam', 'selrow');
        //        var fol = jQuery("#list47").jqGrid('getRowData', id);


        var Cuenta_Bancaria = $("#" + id + "_Cuenta_Bancaria").val();



        if (valor.length != 18) {
            return [false, "El campo debe contener 18 caracteres"];
        }


        if (columnName == "Confirmar Cuenta") {

            if (Cuenta_Bancaria != valor) {
                return [false, "Las Cuentas Bancarias no coinciden"];
            }
            else { //FJCP_10290_CC
                if (Cuenta_Bancaria == "912000000000000000") {
                    return [false, "No se pude usar la cuenta para pago internacional"];
                }
                if (Cuenta_Bancaria == "999000000000000000") {
                    return [false, "No se pude usar la cuenta para pagos tesofe"];
                }

                jQuery("#list47").jqGrid('setCell', id, 'Cuenta_Bancaria_ok', Cuenta_Bancaria);
                jQuery("#list47").jqGrid('setCell', id, 'Confirmar_Cuenta_ok', valor)


            }
        }

        return [true, ""];
    };

    $("#btn_Nuevo").click(function () {
        $("#CatalogoTerceros").modal('hide');
        $("#RegistroTerceros").modal('show');
    });


});




///////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////



function Terceros(ID) {

    $("[id*=ID_row]").val(ID);
    // var nomTercero = jQuery("#list47").jqGrid('getRowData', ID).Nombre_Razon_Social;
    // var RFC = jQuery("#list47").jqGrid('getRowData', ID).RFC;
    //var CodigoCliente = jQuery("#list47").jqGrid('getRowData', ID).CodigoCliente;

    var CodigoCliente = $("#" + ID + "_CodigoCliente").val();
    var RFC = $("#" + ID + "_RFC").val();
    var nomTercero = $("#" + ID + "_Nombre_Razon_Social").val();

    if (CodigoCliente === undefined || CodigoCliente === null) {
        CodigoCliente = jQuery("#list47").jqGrid('getRowData', ID).CodigoCliente;
    }

    if (RFC === undefined || RFC === null) {
        RFC = jQuery("#list47").jqGrid('getRowData', ID).RFC;
    }

    if (nomTercero === undefined || nomTercero === null) {
        nomTercero = jQuery("#list47").jqGrid('getRowData', ID).Nombre_Razon_Social;
    }

    $.ajax({
        url: "../LocalServices/OrdenPagoMasiva.asmx/RecuperaTercero",
        data: "{ 'Nombre': '" + nomTercero + "','RFC':'" + RFC + "','Codigo':'" + CodigoCliente + "'}",
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {



            var mydata = $.parseJSON(result.d);

            if (mydata.length == 0) {
                $("#RegistroTerceros").modal('show');

            }
            else {
                LoadGridCatTerceros(mydata);
                $("#CatalogoTerceros").modal('show');

            }

            return;



        },
        error: function (err) {



        }
    });






};


function selecTercero(rowId) {

    //alert("Alerta JavaScript")
    var codTercero = $("[id*=hidCodTercero]").val();
    var nomTercero = $("[id*=hidNomTercero]").val();
    var rfcTercero = $("[id*=hidrfcTercero]").val();

    jQuery("#list47").jqGrid('setCell', rowId, 'CodigoCliente', codTercero);
    jQuery("#list47").jqGrid('setCell', rowId, 'Nombre_Razon_Social', nomTercero);
    jQuery("#list47").jqGrid('setCell', rowId, 'RFC', rfcTercero);



}

function soloNumeros(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}


function convMayusculas(control) {
    var str = $("[id*=" + control + "]").val();
    var strMayus = str.toUpperCase();
    $("[id*=" + control + "]").val(strMayus);
}



function soloNumeros(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}


function charRFC(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key > 47 && key < 58) {
        return true;
    }

    if (key > 64 && key < 91) {
        return true;
    }

    if (key > 96 && key < 123) {
        return true;
    }

    return false;
}



function validarFecha(control) {
    var dato = $("[id*=" + control + "]").val();
    var fecha = dato.trim();

    try {
        var fecha = fecha.split("/");
        var dia = fecha[0];
        var mes = fecha[1];
        var ano = fecha[2];
        var estado = true;

        if ((dia.length == 2) && (mes.length == 2) && (ano.length == 4)) {
            switch (parseInt(mes)) {
                case 1: dmax = 31; break;
                case 2: if (ano % 4 == 0) dmax = 29; else dmax = 28; break;
                case 3: dmax = 31; break;
                case 4: dmax = 30; break;
                case 5: dmax = 31; break;
                case 6: dmax = 30; break;
                case 7: dmax = 31; break;
                case 8: dmax = 31; break;
                case 9: dmax = 30; break;
                case 10: dmax = 31; break;
                case 11: dmax = 30; break;
                case 12: dmax = 31; break;
            }

            dmax != "" ? dmax : dmax = -1;

            if ((dia >= 1) && (dia <= dmax) && (mes >= 1) && (mes <= 12)) {
                for (var i = 0; i < fecha[0].length; i++) {
                    diaC = fecha[0].charAt(i).charCodeAt(0);
                    (!((diaC > 47) && (diaC < 58))) ? estado = false : '';
                    mesC = fecha[1].charAt(i).charCodeAt(0);
                    (!((mesC > 47) && (mesC < 58))) ? estado = false : '';
                }

            }

            for (var i = 0; i < fecha[2].length; i++) {
                anoC = fecha[2].charAt(i).charCodeAt(0);
                (!((anoC > 47) && (anoC < 58))) ? estado = false : '';
            }
        }
        else estado = false;
        return estado;

    } catch (err) {
        alert("Error fechas");
    }
}



function obtenerEdad() {
    let hoy = new Date();
    var fechaNac = $("[id*=txt_fecNac]").val();
    var fechaNacFto = fechaNac.substring(3, 5) + "/" + fechaNac.substring(0, 2) + "/" + fechaNac.substring(6, 10);
    let fechaNacimiento = new Date(fechaNacFto);
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear();
    let diferenciaMeses = hoy.getMonth() - fechaNacimiento.getMonth();
    if (diferenciaMeses < 0 || (diferenciaMeses === 0 && hoy.getDate() < fechaNacimiento.getDate())) {
        edad--;
    }
    $("[id*=txt_edad]").val(edad)
}




function telefono(e, control) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }


    if (LengthCheck(control)) {
        return true;
    }
    else {
        return false;
    }
}


function LengthCheck(control) {
    var dato = $("[id*=" + control + "]").val();
    var long = dato.length;

    if (long > 9) {
        //fn_MuestraMensaje('Motivo Rechazo', "Ha alcanzado el numero de caracteres permitidos", 0);
        //alert("Caracteres permitidos");
        return false;
    }
    return true;
}


function formatoMoneda(control) {
    var monto = $("[id*=" + control + "]").val();

    var coma = true

    while (coma) {
        if (monto.indexOf(',') !== -1) {
            monto = monto.replace(",", "");
            coma = true
        }
        else {
            coma = false
        }
    }

    monto += '';
    var arrMonto = monto.split('.');
    var entero = arrMonto[0];
    var decimal = arrMonto.length > 1 ? '.' + arrMonto[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(entero)) {
        entero = entero.replace(rgx, '$1' + ',' + '$2');
    }

    var montoFinal = entero + decimal;
    $("[id*=" + control + "]").val(montoFinal);
    //return entero + decimal;
}

function importe(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key == 46) {
        return true;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}



$("body").on('click', '[id*=chkFisica]', function (e) {

    if ($(this).is(':checked')) {
        $("[id*=chkMoral]").prop('checked', false);
        $("[id*=txt_apMat]").prop('disabled', false);
        $("[id*=txt_nombres]").prop('disabled', false);
    }
    else {
        $("[id*=chkFisica]").prop('checked', true);
    }
});


$("body").on('change', "#cmbPagarA", function () {
    //alert($(this).val());

    if ($(this).val() != 10) {
        $("[id*=chkMultipago]").prop('disabled', true);
        $("[id*=txtRFC]").prop('disabled', true);
        $("[id*=txtSiniestro]").prop('disabled', true);

    }
    else {
        $("[id*=chkMultipago]").prop('disabled', false);
        $("[id*=txtRFC]").prop('disabled', false);
        $("[id*=txtSiniestro]").prop('disabled', false);
    }

    

});


$("body").on("click", ".contraer", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "1");
});

//Expandir Ventana
$("body").on("click", ".expandir", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "0");
});

$("body").on('click', '[id*=chkMoral]', function (e) {

    if ($(this).is(':checked')) {
        $("[id*=chkFisica]").prop('checked', false);
        $("[id*=txt_apMat]").prop('disabled', true);
        $("[id*=txt_nombres]").prop('disabled', true);
    }
    else {
        $("[id*=chkMoral]").prop('checked', true);
    }
});



function LoadGridCatTerceros(mydata) {
    var lastsel2;
    var lastSel = -1;
    $("#grdTercero").jqGrid("clearGridData");
    $("#grdTercero").jqGrid("GridUnload")
    try {
        jQuery("#grdTercero").jqGrid({

            data: mydata,
            datatype: "local",

            height: 250,
            width: 200,
            rowNum: 8000,
            rowList: [10, 20, 30],
            colNames: ['Codigo', 'Nombre', 'RFC'],
            colModel: [

                { name: 'cod_tercero', index: 'cod_tercero', width: 50, frozen: false },
                { name: 'nombre', index: 'nombre', width: 250 },
                { name: 'nro_nit', index: 'nro_nit', width: 110 },


            ],




            // pager: "#plist47",
            viewrecords: true,

            autowidth: true,
            shrinkToFit: false,
            forceFit: true,
            loadonce: true,
            multiselect: false,
            rownumbers: true,
            editurl: '../LocalServices/OrdenPagoMasiva.asmx/Apoyo',
            ondblClickRow: function (id, ri, ci) {

                var codTercero = jQuery("#grdTercero").jqGrid('getRowData', id).cod_tercero;
                var nomTercero = jQuery("#grdTercero").jqGrid('getRowData', id).nombre;
                var rfcTercero = jQuery("#grdTercero").jqGrid('getRowData', id).nro_nit;
                var rowId = $("[id*=ID_row]").val()

                jQuery("#list47").jqGrid('setCell', rowId, 'CodigoCliente', codTercero);
                jQuery("#list47").jqGrid('setCell', rowId, 'Nombre_Razon_Social', nomTercero);
                jQuery("#list47").jqGrid('setCell', rowId, 'RFC', rfcTercero);

                ObtenerCuentasDependencias(rowId, codTercero) 

                $("#CatalogoTerceros").modal('hide');


                return;
            },
            ajaxSelectOptions: {
                error: function (xhr, status, error) {

                    alert(error);
                }
            },
            // height: 'auto'
        });

        jQuery("#grdTercero").jqGrid('setFrozenColumns')






        $("#loading").addClass("hidden");
        $("#btn_Guardar").removeClass("hidden");


    } catch (error) {
        console.error(error);
        // expected output: ReferenceError: nonExistentFunction is not defined
        // Note - error messages will vary depending on browser
    }


};

function ObtenerCuentasDependencias(ID, CodCliente) {
    //>VZAVALETA_10290_CC7
    var FolioOnbase = jQuery("#list47").jqGrid('getRowData', ID).FolioOnbaseHidden

    $.ajax({
        url: "../LocalServices/OrdenPagoMasiva.asmx/RecuperarCtasDepend",
        data: "{ 'CodCliente': '" + CodCliente + "', 'FolioOnbase': '" + FolioOnbase + "'}",
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            var mydata = $.parseJSON(result.d);
            if (mydata.length != 0) {
                var vClabe = mydata[0].clabe
                if (vClabe != "") {
                    jQuery("#list47").jqGrid('setCell', ID, 'Cuenta_Bancaria_ok', vClabe);
                    jQuery("#list47").jqGrid('setCell', ID, 'Confirmar_Cuenta_ok', vClabe);
                    $("#" + ID + "_Cuenta_Bancaria").val(vClabe);
                    $("#" + ID + "_Confirmar_Cuenta").val(vClabe);
                }
                else {
                    jQuery("#list47").jqGrid('setCell', ID, 'Cuenta_Bancaria_ok', null);
                    jQuery("#list47").jqGrid('setCell', ID, 'Confirmar_Cuenta_ok', null);
                    $("#" + ID + "_Cuenta_Bancaria").val(null);
                    $("#" + ID + "_Confirmar_Cuenta").val(null);
                }
            }
            return;
        },
        error: function (err) {
        }
    });
};
function RecuperarClasePago(ID, Cpto_pago, Prestador) {
    $.ajax({
        url: "../LocalServices/OrdenPagoMasiva.asmx/RecuperarClasePago",
        data: "{ 'Cpto_pago': '" + Cpto_pago + "', 'sn_prestador': '" + Prestador + "' }",
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            var mydata = $.parseJSON(result.d);
            if (mydata.length != 0) {
                var vCod_clase_pago = mydata[0].cod_clase_pago
                var vClase_pago = mydata[0].clase_pago_desc
                var vcod_cpto_desc = mydata[0].cod_cpto_desc
                jQuery("#list47").jqGrid('setCell', ID, 'Cod_clas_pago', vCod_clase_pago);
                jQuery("#list47").jqGrid('setCell', ID, 'Clase_pago', vClase_pago);


                $("#hid_cpto_pago").val(Cpto_pago);
                $("#hid_cpto_pago_desc").val(vcod_cpto_desc);
            }
            return;
        },
        error: function (err) {
        }
    });
};
