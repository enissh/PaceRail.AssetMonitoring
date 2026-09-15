#!/bin/bash
# PostgreSQL Automated Backup Script (Retains dumps for 14 days)

BACKUP_DIR="/var/backups/pgadmin_dumps"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
DATABASE_NAME="PaceRailDb"
DB_USER="postgres"

# Create backup directory if it doesn't exist
mkdir -p $BACKUP_DIR

# Perform binary database export
echo "Starting PostgreSQL backup for $DATABASE_NAME..."
pg_dump -U $DB_USER -d $DATABASE_NAME -F c -f "$BACKUP_DIR/${DATABASE_NAME}_${TIMESTAMP}.dump"

# Cleanup: remove backups older than 14 days
find $BACKUP_DIR -type f -name "*.dump" -mtime +14 -delete

echo "Backup completed successfully: ${DATABASE_NAME}_${TIMESTAMP}.dump"