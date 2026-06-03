import os


def copy_files_to_single_txt(source_folder, output_file):
    """
    Recursively finds all .html, .scss, and .ts files in source_folder
    and writes their contents into a single output_file.
    """
    target_extensions = ('.html', '.scss', '.ts')

    with open(output_file, 'w', encoding='utf-8') as outfile:
        for root, dirs, files in os.walk(source_folder):
            for file in files:
                if file.endswith(target_extensions) and not file.endswith('.spec.ts'):
                    file_path = os.path.join(root, file)

                    # Write a header to distinguish files in the output
                    outfile.write(f"--- BEGIN FILE: {file_path} ---\n")

                    try:
                        with open(file_path, 'r', encoding='utf-8') as infile:
                            outfile.write(infile.read())
                            outfile.write("\n\n")
                    except Exception as e:
                        outfile.write(f"Error reading file: {e}\n")

    print(f"Successfully copied contents to {output_file}")


# Usage Configuration
# Replace '.' with your specific folder path if needed
source_directory = './frontend/src/'
output_filename = 'combined_code.txt'

if __name__ == "__main__":
    copy_files_to_single_txt(source_directory, output_filename)
