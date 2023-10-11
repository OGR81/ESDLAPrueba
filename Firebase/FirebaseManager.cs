using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using MESBG.Models;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace MESBG.Firebase
{
    public class FirebaseManager
    {
        private readonly FirestoreDb _db;
        private readonly FirebaseAuth? _auth;


        public FirebaseManager(FirestoreDb db, string credentialFilePath)
        {
            // Verificar que la ruta del archivo de credenciales no esté vacía
            if (string.IsNullOrEmpty(credentialFilePath))
            {
                throw new ArgumentException("La ruta del archivo de credenciales no puede estar vacía.");
            }
                        
            _db = db;
                                    
        }

        public async Task<List<Evento>> GetEventos()
        {
            var eventosList = new List<Evento>();
            Query allEventosQuery = _db.Collection("eventos");
            QuerySnapshot allEventosQuerySnapshot = await allEventosQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allEventosQuerySnapshot.Documents)
            {
                Evento evento = documentSnapshot.ConvertTo<Evento>();
                evento.Id = documentSnapshot.Id;
                eventosList.Add(evento);
            }

            return eventosList;
        }

        public async Task<Evento> GetProximoEventoAsync()
        {
            var eventosList = await GetEventos();
            if (eventosList.Count > 0)
            {
                // Ordena la lista de eventos por fecha en orden descendente (el evento más reciente primero)
                eventosList = eventosList.OrderByDescending(e => e.Fecha).ToList();
                return eventosList.First();
            }
            return null;
        }

        public async Task<List<Evento>> GetEventosPasados()
        {
            var eventosList = await GetEventos();
            if (eventosList.Count > 0)
            {
                // Ordena la lista de eventos por fecha en orden ascendente (eventos más antiguos primero)
                eventosList = eventosList.OrderBy(e => e.Fecha).ToList();
                return eventosList;
            }
            return new List<Evento>();
        }


        public async Task CrearDocEvento(string coleccion, Evento evento)
        {            
            CollectionReference collectionRef = _db.Collection(coleccion);

            try
            {
                // Convierte la imagen a base64 y luego guárdala como una cadena
                string imagenBase64 = ConvertImageToBase64(evento.Imagen);

                var eventoSerialized = new
                {
                    Titulo = evento.Titulo,
                    Descripcion = evento.Descripcion,
                    Imagen = imagenBase64, // Almacena la imagen como base64
                    Fecha = evento.Fecha.ToString("yyyy-MM-dd"),
                    Hora = evento.Hora.ToString("HH:mm:ss"),
                    PeriodoInscripcion = evento.PeriodoInscripcion.ToString("yyyy-MM-dd"),
                };

                await collectionRef.AddAsync(eventoSerialized);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al añadir el evento a Firebase: " + ex.Message);
                throw;
            }
        }

        private string ConvertImageToBase64(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    return Convert.ToBase64String(imageBytes);
                }
                return null; // Devuelve null si la ruta de la imagen no existe o si ocurre algún error.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir la imagen a base64: " + ex.Message);
                throw;
            }
        }


        public async Task ActualizarDocEvento(string coleccion, string documentoId, Dictionary<string, object> updates)
        {
            // Obtener una referencia al documento que deseas actualizar
            DocumentReference documentRef = _db.Collection(coleccion).Document(documentoId);

            // Realizar la actualización
            await documentRef.UpdateAsync(updates);
        }

        public async Task <List<Participante>> GetParticipantes()
        {
            // Obtener una referencia a la colección
            var participantesList = new List<Participante>();
            Query allParticipantesQuery = _db.Collection("participantes");
            QuerySnapshot allParticipantesQuerySnapshot = await allParticipantesQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allParticipantesQuerySnapshot.Documents) 
            {
                Participante participante = new Participante
                {
                    Id = documentSnapshot.Id,
                    Nick = documentSnapshot.GetValue<string>("Nick"),
                    Nombre = documentSnapshot.GetValue<string>("Nombre"),
                    Bando = documentSnapshot.GetValue<string>("Bando"),
                    ListaEnviada = documentSnapshot.GetValue<string>("ListaEnviada"),
                    PagoAbonado = documentSnapshot.GetValue<string>("PagoAbonado")
                };
                

                participantesList.Add(participante);

            }

            return participantesList;
                
        }

        public async Task<List<Participante>> GetParticipantesBynick(string nick)
        {
            // Obtener una referencia a la colección
            var participantesList = new List<Participante>();
            Query participantesQuery = _db.Collection("participantes").WhereEqualTo("Nick", nick);
            QuerySnapshot participantesQuerySnapshot = await participantesQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in participantesQuerySnapshot.Documents)
            {
                Participante participante = documentSnapshot.ConvertTo<Participante>();
                participante.Id = documentSnapshot.Id;

                participantesList.Add(participante);

            }

            return participantesList;

        }
        // Método para eliminar un documento de una colección
        public async Task EliminarDocParticipante(string coleccion, string idParticipante)
        {
            Console.WriteLine("El id es: " + idParticipante);

            // Obtener una referencia al documento que deseas eliminar
            DocumentReference documentRef = _db.Collection(coleccion).Document(idParticipante);

            // Eliminar participante de Firebase
            await documentRef.DeleteAsync();
        }

        public async Task CrearDocParticipante(string coleccion, Participante participante)
        {
            // Obtener una referencia a la colección
            CollectionReference collectionRef = _db.Collection(coleccion);

            try
            {
                // Agregar el nuevo participante a Firebase Firestore
                await collectionRef.AddAsync(participante);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir al agregar el documento
                Console.WriteLine("Error al agregar el participante a Firebase: " + ex.Message);
                throw; // Puedes decidir cómo manejar esto según las necesidades de tu aplicación
            }
        }
                
        // Método para actualizar un documento existente en una colección
        public async Task ActualizarDocParticipante(string coleccion, string documentoId, Dictionary<string, object> updates)
        {
            // Obtener una referencia al documento que deseas actualizar
            DocumentReference documentRef = _db.Collection(coleccion).Document(documentoId);

            // Realizar la actualización
            await documentRef.UpdateAsync(updates);
        }

        public async Task<List<Puntuacion>> GetPuntuaciones()
        {
            // Obtener una referencia a la colección
            var puntuacionesList = new List<Puntuacion>();
            Query allPuntuacionesQuery = _db.Collection("puntuaciones");
            QuerySnapshot allPuntuacionesQuerySnapshot = await allPuntuacionesQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allPuntuacionesQuerySnapshot.Documents)
            {
                Puntuacion puntuacion = new Puntuacion
                {
                    Id = documentSnapshot.Id,
                    Nick = documentSnapshot.GetValue<string>("Nick"),
                    PuntoPartida = documentSnapshot.GetValue<int>("PuntoPartida"),
                    PuntosVictoriaObtenidos = documentSnapshot.GetValue<int>("PuntosVictoriaObtenidos"),
                    PuntosVictoriaCedidos = documentSnapshot.GetValue<int>("PuntosVictoriaCedidos"),
                    LiderAbatido = documentSnapshot.GetValue<int>("LiderAbatido"),
                    Ronda = documentSnapshot.GetValue<int>("Ronda")
                };
                puntuacionesList.Add(puntuacion);
            }

            return puntuacionesList;

        }

        public async Task<List<Puntuacion>> GetPuntuacionesBynick(string nick)
        {
            // Obtener una referencia a la colección
            var puntuacionesList = new List<Puntuacion>();
            Query puntuacionesQuery = _db.Collection("puntuaciones").WhereEqualTo("Nick", nick);
            QuerySnapshot puntuacionesQuerySnapshot = await puntuacionesQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in puntuacionesQuerySnapshot.Documents)
            {
                Puntuacion puntuacion = documentSnapshot.ConvertTo<Puntuacion>();
                puntuacion.Id = documentSnapshot.Id;

                puntuacionesList.Add(puntuacion);

            }

            return puntuacionesList;

        }

        public async Task CrearDocPuntuacion(string coleccion, Puntuacion puntuacion)
        {
            // Obtener una referencia a la colección
            CollectionReference collectionRef = _db.Collection(coleccion);

            try
            {
                // Agregar el nuevo participante a Firebase Firestore
                await collectionRef.AddAsync(puntuacion);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir al agregar el documento
                Console.WriteLine("Error al añadir la puntuación a Firebase: " + ex.Message);
                throw; // Puedes decidir cómo manejar esto según las necesidades de tu aplicación
            }
        }

        public async Task EliminarDocPuntuacion(string coleccion, string idPuntuacion)
        {
            Console.WriteLine("El id es: " + idPuntuacion);

            // Obtener una referencia al documento que deseas eliminar
            DocumentReference documentRef = _db.Collection(coleccion).Document(idPuntuacion);

            // Eliminar puntuación de Firebase
            await documentRef.DeleteAsync();
        }

        public async Task ActualizarDocPuntuacion(string coleccion, string documentoId, Dictionary<string, object> updates)
        {
            // Obtener una referencia al documento que deseas actualizar
            DocumentReference documentRef = _db.Collection(coleccion).Document(documentoId);

            // Realizar la actualización
            await documentRef.UpdateAsync(updates);
        }

        public async Task<List<ClasificacionJugador>> GetClasificacion()
        {
            // Obtener una referencia a la colección
            var clasificacionList = new List<ClasificacionJugador>();
            Query allClasificacionQuery = _db.Collection("clasificacion");
            QuerySnapshot allClasificacionQuerySnapshot = await allClasificacionQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allClasificacionQuerySnapshot.Documents)
            {
                ClasificacionJugador clasificacionJugador = new ClasificacionJugador
                {
                    Id = documentSnapshot.Id,
                    Nick = documentSnapshot.GetValue<string>("Nick"),
                    TotalPtosVictoriaObtenidos = documentSnapshot.GetValue<int>("TotalPtosVictoriaObtenidos"),
                    TotalPtosVictoriaCedidos = documentSnapshot.GetValue<int>("TotalPtosVictoriaCedidos"),
                    TotalDifPtosVictoria = documentSnapshot.GetValue<int>("TotalDifPtosVictoria"),
                    TotalLideresAbatidos = documentSnapshot.GetValue<int>("TotalLideresAbatidos"),
                    Total = documentSnapshot.GetValue<int>("Total")
                };
                clasificacionList.Add(clasificacionJugador);
            }

            return clasificacionList;

        }

        public async Task<List<ClasificacionJugador>> GetClasificacionBytotal(int total)
        {
            // Obtener una referencia a la colección
            var clasificacionList = new List<ClasificacionJugador>();
            Query clasificacionQuery = _db.Collection("clasificacion").WhereEqualTo("Total", total);
            QuerySnapshot clasificacionQuerySnapshot = await clasificacionQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in clasificacionQuerySnapshot.Documents)
            {
                ClasificacionJugador clasificacionJugador = documentSnapshot.ConvertTo<ClasificacionJugador>();
                clasificacionJugador.Id = documentSnapshot.Id;

                clasificacionList.Add(clasificacionJugador);

            }

            return clasificacionList;

        }

    }
}















