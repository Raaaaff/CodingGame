import express from 'express';
import mysql from 'mysql2';
const app = express();
const port = 3000;

app.use(express.json());

const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'clinic'
});

db.connect(err => {
    if (err) throw err;
    console.log('Connecté à la base de données MySQL');
});

app.get('/owners', (req, res) => {
    db.query('SELECT * FROM owners', (err, results) => {
        if (err) return res.status(500).send(err);
        res.json(results);
    });
});

app.listen(port, () => {
    console.log(`Serveur démarré sur http://localhost:${port}`);
});

