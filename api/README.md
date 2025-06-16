
# Clinic API (Node.js)

Cette API permet de gérer une clinique vétérinaire avec des CRUD sur trois entités : `owners`, `animals` et `appointments`.

## 🧰 Installation

1. Cloner ce dépôt dans un dossier local
2. Importer le fichier `clinic.sql` dans votre MySQL via XAMPP
3. Installer les dépendances :
```bash
npm install
```
4. Lancer le serveur :
```bash
node index.js
```

## 🧪 Exemple de route

- `GET /owners`
- `POST /animals`
- `PUT /appointments/:id`
- `DELETE /owners/:id`

## 📦 Technologies

- Node.js
- Express
- MySQL
