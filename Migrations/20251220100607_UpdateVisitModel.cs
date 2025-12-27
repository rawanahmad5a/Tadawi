using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tadawi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVisitModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_PatientFiles_PatientFileId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "VisitNotes",
                table: "Visits",
                newName: "VisitType");

            migrationBuilder.AlterColumn<int>(
                name: "PatientFileId",
                table: "Patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_PatientFiles_PatientFileId",
                table: "Patients",
                column: "PatientFileId",
                principalTable: "PatientFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_PatientFiles_PatientFileId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "VisitType",
                table: "Visits",
                newName: "VisitNotes");

            migrationBuilder.AlterColumn<int>(
                name: "PatientFileId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_PatientFiles_PatientFileId",
                table: "Patients",
                column: "PatientFileId",
                principalTable: "PatientFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
