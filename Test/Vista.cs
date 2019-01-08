using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Vista
    {
        //*************** HILO PRINCIPAL ******************
        //        Task.Factory.StartNew(() =>
        //            {
        //                try
        //                {
        //                    _isTransactionCompleted = false;
        //                    HopperBusiness.Cobrar();
        //                }
        //                catch (CambioException ce)
        //                {
        //                    _isTransactionCompleted = true;
        //                    if (HopperBusiness != null)
        //                        HopperBusiness.Dispose();
        //                    HopperBusiness = null;
        //                    CashPaymentErrorCambioAction(ce);
        //}
        //                catch (Exception ex)
        //                {
        //                    _isTransactionCompleted = true;
        //                    if (HopperBusiness != null)
        //                        HopperBusiness.Dispose();
        //                    HopperBusiness = null;
        //                    CashPaymentErrorGeneralAction(ex);
        //                }
        //            });

        //*************************** FUNCION COBRAR ***************************
        public void Cobrar()
        {
            //_transaccionTerminada = false;
            //double count_actual = 0;
            //var puedeRecibir = false;
            //_cajaBusiness = new CajaBusiness();
            //if (_billAcceptor != null)
            //    _billAcceptor.open();
            //if (_hopperAcceptor != null)
            //    _hopperAcceptor.open();
            //while (!_transaccionTerminada)
            //{
            //    if (_hopperAcceptor != null && !_hopperAcceptor.isConnection())
            //    {
            //        continue;
            //    }
            //    if (_billAcceptor != null && !_billAcceptor.isConnection())
            //    {
            //        continue;
            //    }
            //    if (_hopperAcceptor != null && _hopperAcceptor.isConnection())
            //    {
            //        _hopperAcceptor.enable();
            //        if (_billAcceptor != null && _billAcceptor.isConnection())
            //        {
            //            _billAcceptor.enable();
            //            if (!puedeRecibir)
            //            {
            //                PuedeRecibirEfectivoAction(true);
            //            }
            //            puedeRecibir = true;
            //            double[] recibidoCoin = _hopperAcceptor.getCashDesposite();
            //            double[] recibidoBill = _billAcceptor.getCashDesposite();

            //            if (recibidoCoin.Length > 0 && count_actual != recibidoCoin[1])
            //            {
            //                _cajaBusiness.RecibirDinero("Monedas" + recibidoCoin[0]);
            //                CashPaymentRecibidoAction(RecibidoTransaccion.Total);
            //                count_actual = recibidoCoin[1];
            //            }
            //            if (recibidoBill.Length > 0 && recibidoBill[0] > 0)
            //            {
            //                _cajaBusiness.RecibirDinero("Billetes" + recibidoBill[0]);
            //                CashPaymentRecibidoAction(RecibidoTransaccion.Total);
            //            }
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            //try
            //{
            //    if (_billAcceptor != null && _billAcceptor.isConnection())
            //        _billAcceptor.disable();
            //    if (_hopperAcceptor != null && _hopperAcceptor.isConnection())
            //        _hopperAcceptor.close();
            //}
            //catch (Exception)
            //{
            //}
        }

        //*************************** FUNCION DISPOSE ***************************
        public void Dispose()
        {
            //try
            //{
            //    _billAcceptor.close();
            //    _billDispenser.close();
            //    _hopperAcceptor.close();
            //    _hopperDispenser.close();
            //}
            //catch (Exception)
            //{
            //Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector", "Error al Cerrar Dispositivos: " + ex.ToString());
            //}
            //_hopperAcceptor = null;
            //_hopperDispenser = null;
            //_billAcceptor = null;
            //_billDispenser = null;
            //_factoryDevice = null;
        }

        //*************************** FUNCION ACEPTAR ***************************
        public decimal Aceptar()
        {
            //if (RecibidoTransaccion == null)
            //{
            //    _transaccionTerminada = true;
            //    return 0.0m;
            //}

            //try
            //{
            //    _transaccionTerminada = true;
            //    var cambioPorDar = RecibidoTransaccion.Total - _total;
            //    var cambioPuedeDar = _cajaBusiness.CalcularCambio(cambioPorDar);
            //    var transaccion = _cajaBusiness.CalcularTransaccion(_idTransaccionVista, _total, "OK",
            //        RecibidoTransaccion, cambioPuedeDar, null);
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //        "Transaccion Correcta: " + JsonConvert.SerializeObject(transaccion));
            //    DarCambio(cambioPuedeDar);
            //    _cajaBusiness.SaveTransaccion(transaccion);
            //    return cambioPuedeDar.Total;
            //}
            //catch (CashException cashEx)
            //{
            //    throw ThrowCashException(cashEx);
            //}
            //catch (CambioException ce)
            //{
            //    _cajaBusiness.ResetCaja();
            //    _transaccionTerminada = true;
            //    var transaccion = _cajaBusiness.CalcularTransaccion(_idTransaccionVista, _total, "Error",
            //        RecibidoTransaccion, ce.CambioPuedeEntregar,
            //        RecibidoTransaccion.Total - ce.CambioPuedeEntregar.Total);
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //        "Transacción con Error: " + JsonConvert.SerializeObject(transaccion));
            //    try
            //    {
            //        DarCambio(ce.CambioPuedeEntregar);
            //    }
            //    catch (CashException cashEx)
            //    {
            //        throw ThrowCashException(cashEx);
            //    }
            //    _cajaBusiness.SaveTransaccion(transaccion);
            //    var noEntregado = transaccion.CambioNoEntregado ?? 0;
            //    var cashExceptionAux =
            //        new CambioException(ce.Message + noEntregado.ToString("c2"), ce.CambioPuedeEntregar);
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //        "Cambio Puede Entregar: " + JsonConvert.SerializeObject(ce.CambioPuedeEntregar));
            //    throw cashExceptionAux;
            //}
            //catch (Exception ex)
            //{
            //    _transaccionTerminada = true;
            //    throw ex;
            //}
            //finally
            //{
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector", "Desconectando Dispositivos al Aceptar");
            //}

            return 0.0m;
        }

        //*************************** EVENTO CLICK ACEPTAR ***************************
        private void btnAceptar_Click(object sender /*, RoutedEventArgs e*/)
        {
            //Thread thread = new Thread(new ThreadStart(delegate ()
            //{
            //    Thread.Sleep(300); // this is important ...
            //    try
            //    {
            //        this.Dispatcher.BeginInvoke(DispatcherPriority.Send,
            //            new Action(delegate ()
            //            {
            //                lbIngrese.Visibility = Visibility.Hidden;
            //                lbEspere.Content = "Finalizando Transacción";
            //                lbEspere.Visibility = Visibility.Visible;
            //            }));
            //    }
            //    catch { }
            //}));
            //thread.Name = "thread-UpdateAceptar";
            //thread.Start();
            //_isTransactionCompleted = true;
            //btnAceptar.IsEnabled = false;
            //btnCancelar.IsEnabled = false;
            //Thread threadCambio = new Thread(new ThreadStart(delegate ()
            //{
            //    Thread.Sleep(300); // this is important ...
            //    var cambioEntregado = 0.0m;
            //    try
            //    {
            //        cambioEntregado = HopperBusiness.Aceptar();
            //        var total = HopperBusiness.RecibidoTransaccion == null
            //            ? 0.0m
            //            : HopperBusiness.RecibidoTransaccion.Total;
            //        _cashPaymentResponse = new CashPaymentResponse()
            //        {
            //            Result = DialogResult.OK,
            //            AmountCents = _connectorRequest.AmountCents,
            //            ReceivedCents = Convert.ToInt32(total * 100),
            //            ChangeCents = Convert.ToInt32(cambioEntregado * 100),
            //        };
            //        if (HopperBusiness != null)
            //            HopperBusiness.Dispose();
            //        HopperBusiness = null;
            //        CashPaymentResponseAction(_cashPaymentResponse);
            //    }
            //    catch (CambioException ce)
            //    {
            //        if (HopperBusiness != null)
            //            HopperBusiness.Dispose();
            //        HopperBusiness = null;
            //        CashPaymentErrorCambioAction(ce);
            //    }
            //    catch (Exception ex)
            //    {
            //        if (HopperBusiness != null)
            //            HopperBusiness.Dispose();
            //        HopperBusiness = null;
            //        CashPaymentErrorGeneralAction(ex);
            //    }
            //}));
            //threadCambio.Name = "thread-UpdateCambio";
            //threadCambio.Start();
        }

        //*************************** FUNCION CANCELAR ***************************
        public decimal Cancelar()
        {
            //if (RecibidoTransaccion == null)
            //{
            //    _transaccionTerminada = true;
            //    return 0.0m;
            //}

            //try
            //{
            //    _transaccionTerminada = true;
            //    var cambioPuedeDar = _cajaBusiness.CalcularCambio(RecibidoTransaccion.Total);
            //    var transaccion = _cajaBusiness.CalcularTransaccion(_idTransaccionVista, _total, "Cancelada",
            //        RecibidoTransaccion, cambioPuedeDar, null);
            //    DarCambio(cambioPuedeDar);
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //        "Transaccion Cancelada: " + JsonConvert.SerializeObject(transaccion));
            //    _cajaBusiness.SaveTransaccion(transaccion);
            //    return cambioPuedeDar.Total;
            //}
            //catch (CashException cashEx)
            //{
            //    throw ThrowCashException(cashEx);
            //}
            //catch (CambioException ce)
            //{
            //    _cajaBusiness.ResetCaja();
            //    _transaccionTerminada = true;
            //    var transaccion = _cajaBusiness.CalcularTransaccion(_idTransaccionVista, _total, "Error",
            //        RecibidoTransaccion, ce.CambioPuedeEntregar,
            //        RecibidoTransaccion.Total - ce.CambioPuedeEntregar.Total);
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //        "Transacción con Error: " + JsonConvert.SerializeObject(transaccion));
            //    try
            //    {
            //        DarCambio(ce.CambioPuedeEntregar);
            //    }
            //    catch (CashException cashEx)
            //    {
            //        Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //            "Línea: 200");
            //        throw ThrowCashException(cashEx);
            //    }
            //    _cajaBusiness.SaveTransaccion(transaccion);
            //    var noEntregado = transaccion.CambioNoEntregado ?? 0;
            //    var cashExceptionAux =
            //        new CambioException(ce.Message + noEntregado.ToString("c2"), ce.CambioPuedeEntregar);
            //    throw cashExceptionAux;
            //}
            //catch (Exception ex)
            //{
            //    _transaccionTerminada = true;
            //    throw ex;
            //}
            //finally
            //{
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector", "Desconectando Dispositivos al Cancelar");
            //}

            return 0.0m;
        }

        //*************************** FUNCION CANCELAR TRANSACCIÓN ***************************
        private void CancelarTransaccion()
        {
            //Thread thread = new Thread(new ThreadStart(delegate ()
            //{
            //    Thread.Sleep(300); // this is important ...
            //    try
            //    {
            //        this.Dispatcher.BeginInvoke(
            //            new Action(delegate ()
            //            {
            //                btnAceptar.IsEnabled = false;
            //                btnCancelar.IsEnabled = false;
            //                lbIngrese.Visibility = Visibility.Hidden;
            //                lbEspere.Content = "Cancelando Transacción";
            //                lbEspere.Visibility = Visibility.Visible;
            //            }));
            //    }
            //    catch { }
            //}));
            //thread.Name = "thread-Cancelando";
            //thread.Start();
            //_isTransactionCompleted = true;
            //Thread threadCambio = new Thread(new ThreadStart(delegate ()
            //{
            //    Thread.Sleep(300); // this is important ...
            //    var cambioEntregado = 0.0m;
            //    try
            //    {
            //        cambioEntregado = HopperBusiness.Cancelar();
            //        var total = HopperBusiness.RecibidoTransaccion == null
            //            ? 0.0m
            //            : HopperBusiness.RecibidoTransaccion.Total;
            //        _cashPaymentResponse = new CashPaymentResponse()
            //        {
            //            Result = DialogResult.Cancel,
            //            AmountCents = _connectorRequest.AmountCents,
            //            ReceivedCents = Convert.ToInt32(total * 100),
            //            ChangeCents = Convert.ToInt32(cambioEntregado * 100),
            //        };
            //        if (HopperBusiness != null)
            //            HopperBusiness.Dispose();
            //        HopperBusiness = null;
            //        CashPaymentResponseAction(_cashPaymentResponse);
            //    }
            //    catch (CambioException ce)
            //    {
            //        if (HopperBusiness != null)
            //            HopperBusiness.Dispose();
            //        HopperBusiness = null;
            //        CashPaymentErrorCambioAction(ce);
            //    }
            //    catch (Exception ex)
            //    {
            //        if (HopperBusiness != null)
            //            HopperBusiness.Dispose();
            //        HopperBusiness = null;
            //        CashPaymentErrorGeneralAction(ex);
            //    }
            //}));
            //threadCambio.Name = "thread-UpdateCambio";
            //threadCambio.Start();
        }

        //*************************** FUNCION DAR CAMBIO ***************************
        public void DarCambio(/*CambioBD cambio*/)
        {
            //try
            //{
            //    var properties = cambio.GetType().GetProperties();
            //    var arrayBilletes = _cajaBusiness.Caja.DenominacionesCambio.Where(dc => dc.StartsWith("Billetes"))
            //        .ToList();
            //    var arrayMonedas = _cajaBusiness.Caja.DenominacionesCambio.Where(dc => dc.StartsWith("Monedas"))
            //        .ToList();
            //    int[] returnCoins = new int[arrayMonedas.Count];
            //    int[] returnBill = new int[arrayBilletes.Count];
            //    for (int i = 0; i < arrayBilletes.Count; i++)
            //        returnBill[i] = 0;
            //    foreach (var property in properties)
            //    {
            //        if (property.Name.StartsWith("Monedas"))
            //        {
            //            var cantidadMonedas = (int?)property.GetValue(cambio);
            //            if (cantidadMonedas != null)
            //                returnCoins[arrayMonedas.IndexOf(property.Name)] = cantidadMonedas.Value;
            //        }

            //        if (property.Name.StartsWith("Billetes"))
            //        {
            //            var cantidadBilletes = (int?)property.GetValue(cambio);
            //            if (cantidadBilletes != null)
            //                returnBill[arrayBilletes.IndexOf(property.Name)] = cantidadBilletes.Value;
            //        }
            //    }

            //    if (returnCoins.Sum() > 0)
            //    {
            //        _hopperDispenser.open();
            //        Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //            "Cambio de Monedas: " + JsonConvert.SerializeObject(returnCoins));
            //        _hopperDispenser.returnCash(returnCoins);
            //    }

            //    if (returnBill.Sum() > 0)
            //    {
            //        _billDispenser.open();
            //        _billDispenser.enable();
            //        Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector",
            //            "Cambio de Billetes: " + JsonConvert.SerializeObject(returnBill));
            //        _billDispenser.returnCash(returnBill);
            //    }
            //}
            //catch (CashException ce)
            //{
            //    throw ce;
            //}
            //catch (CambioException ce)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector", "Error al dar cambio: " + ex.ToString());
            //    var cambioPuedeEntregar = new CambioBD();
            //    cambioPuedeEntregar.IdTransaccion = _idTransaccionVista;
            //    var cambioException = new CambioException("Error al entregar el cambio", cambioPuedeEntregar);
            //    throw cambioException;
            //}

            //Vista.Common.Config.LogMessage("visCMXHopperSCPaymentConnector", "Cantidad devuelta en billetes: " + cambio.TotalBilletes + ", en monedas: " + cambio.TotalMonedas);
        }
    }
}
