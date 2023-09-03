using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using ESDLAPrueba.Models;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESDLAPrueba.Firebase
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

        // Método para leer un documento por su ID
        public async Task<DocumentSnapshot> LeerDocumento(string coleccion, string documentoId)
        {
            // Obtener una referencia a la colección
            CollectionReference collectionRef = _db.Collection(coleccion);

            // Obtener el documento por su ID
            DocumentSnapshot document = await collectionRef.Document(documentoId).GetSnapshotAsync();

            return document;
        }

        public async Task <List<Participante>> GetParticipantes()
        {
            // Obtener una referencia a la colección
            var participantesList = new List<Participante>();
            Query allParticipantesQuery = _db.Collection("participantes");
            QuerySnapshot allParticipantesQuerySnapshot = await allParticipantesQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allParticipantesQuerySnapshot.Documents) 
            {
                Participante participante = documentSnapshot.ConvertTo<Participante>();
                participante.Id = documentSnapshot.Id;

                participantesList.Add(participante);

            }

            return participantesList;
                
        }

        // Método para eliminar un documento de una colección
        public async Task EliminarDocumento(string coleccion, string idParticipante)
        {
            Console.WriteLine("El id es: " + idParticipante);

            // Obtener una referencia al documento que deseas eliminar
            DocumentReference documentRef = _db.Collection(coleccion).Document(idParticipante);

            // Eliminar participante de Firebase
            await documentRef.DeleteAsync();
        }

        public async Task CrearDocumento(string coleccion, Participante participante)
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
        public async Task ActualizarDocumento(string coleccion, string documentoId, Dictionary<string, object> updates)
        {
            // Obtener una referencia al documento que deseas actualizar
            DocumentReference documentRef = _db.Collection(coleccion).Document(documentoId);

            // Realizar la actualización
            await documentRef.UpdateAsync(updates);
        }

        

        
    }
}















