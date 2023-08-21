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
                participantesList.Add(documentSnapshot.ConvertTo<Participante>());

            }

            return participantesList;
                
        }

        public async Task DeleteParticipante (int id)
        {
            Console.WriteLine("El id es: " + id);
            Query docRef = _db.Collection("participantes").WhereEqualTo("Id", id).Limit(1);
            QuerySnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Count > 0)
            {
                DocumentReference participanteRef = _db.Collection("participantes").Document(snapshot.ElementAt(0).Id);
                await participanteRef.DeleteAsync();
            }

        }

        // Método para crear un nuevo documento en una colección
        public async Task<string> CrearDocumento(string coleccion, object datos)
        {
            // Obtener una referencia a la colección
            CollectionReference collectionRef = _db.Collection(coleccion);

            // Crear un nuevo documento con un ID automático
            DocumentReference documentRef = await collectionRef.AddAsync(datos);

            // Obtener el ID del nuevo documento
            string newDocumentId = documentRef.Id;

            return newDocumentId;
        }

        // Método para actualizar un documento existente en una colección
        public async Task ActualizarDocumento(string coleccion, string documentoId, Dictionary<string, object> updates)
        {
            // Obtener una referencia al documento que deseas actualizar
            DocumentReference documentRef = _db.Collection(coleccion).Document(documentoId);

            // Realizar la actualización
            await documentRef.UpdateAsync(updates);
        }

        // Método para eliminar un documento de una colección
        public async Task EliminarDocumento(string coleccion, string documentoId)
        {
            // Obtener una referencia al documento que deseas eliminar
            DocumentReference documentRef = _db.Collection(coleccion).Document(documentoId);

            // Realizar la eliminación
            await documentRef.DeleteAsync();
        }

        
    }
}















