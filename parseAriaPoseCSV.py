import csv

input_path = "again_closed_loop_trajectory_TRIMMED.csv" # edit this line to specify the path of the *original* CSV file you want to clean
output_path = "again_office_cleaned_test1.csv" # edit this line to specify the path for the *cleaned* CSV file which be outputted by running this script

columns_to_replace = [
    # You could certainly specify more or less columns to replace, I just quickly created this to make the CSV file smaller
    # For Aria 6 DoF headpose, I only need the following columns:
        # 3 DoF translation: tx_world_device, ty_world_device, tz_world_device
            # will convert to Vector3 objects when parsed in Unity
        # 3 DoF rotation: qx_world_device, qy_world_device, qz_world_device, qw_world_device
            # will convert to Quaternion objects when parsed in Unity

    "graph_uid", "tracking_timestamp_us", "utc_timestamp_ns",
    "gravity_x_world", "gravity_y_world", "gravity_z_world",
    "quality_score", "geo_available",
    "tx_ecef_device", "ty_ecef_device", "tz_ecef_device",
    "qx_ecef_device", "qy_ecef_device", "qz_ecef_device", "qw_ecef_device"
]

with open(input_path, 'r') as infile, open(output_path, 'w', newline='') as outfile:
    reader = csv.DictReader(infile)
    fieldnames = reader.fieldnames

    writer = csv.DictWriter(outfile, fieldnames=fieldnames)
    writer.writeheader()

    for row in reader:
        # if statement to skip any rows that are empty
        if not any(row.values()):
            continue

        for col in columns_to_replace:
            if col in row and row[col].strip() != "":
                row[col] = "1" # I will make this script far more efficient in the future, but for now I am just replacing the very large numbers (i.e. previously requiring double datatypes) with '1'

        writer.writerow(row)

print(f"Columns {columns_to_replace} values replaced with 1 in {output_path}")

# Instructions to run script:
    # 1. Open new terminal
    # 2. Change directory (cd) to script path
    # 3. Run the following command in the terminal: python3 parseAriaClosedTrajectoryCSV.py
# Instructions to view cleaned CSV file:
    # 1. Change directory (cd) to same path as your output_path variable
    # 2. Open in application of choice (e.g. Microsoft Excel)
