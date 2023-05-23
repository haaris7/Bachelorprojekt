import pandas as pd
import csv

dataset = "../../Data/GazeData2.csv"
header_names = ['GazeTime', 'Region', 'Target', 'PosX', 'PosY']
df = pd.read_csv(dataset, delimiter=';', names=header_names, header=0, index_col=False)
grouped_data = df.groupby('Region')
grouped_data_R_T = df.groupby(['Region', 'Target'])


def sum(arr):
    ret = 0
    for i in arr:
        ret += i
    return ret


def PercFixInside():
    def Compute(data, region, interest):
        ret = pd.DataFrame()
        target_names = grouped_data.get_group(region)['Target'].unique()
        gaze = [0] * len(target_names)  # Initialize list with zeros
        perc = [0] * len(target_names)
        ret['Target'] = target_names

        for j, i in enumerate(target_names):
            gaze[j] = data.get_group((region, i))[interest].sum()
        tot = sum(gaze)

        for i, j in enumerate(gaze):
            perc[i] = (j / tot) * 100

        ret['GazeTime'] = gaze
        ret['Percentage'] = perc
        return ret
    for region in grouped_data.groups:
        print("Region: " + str(region))
        data = Compute(grouped_data_R_T, region, 'GazeTime')
        print(data)
        print()
def NFix(data_file, threshold, ShowPercentage = False):
    if(threshold < 0):
        threshold = abs(threshold)
    nfix = 0  # Initialize the NFix counter
    with open(data_file, 'r') as file:
        reader = csv.reader(file, delimiter=';')
        header = next(reader)  # Skip the header
        counter = 0;
        pos_x, pos_y = None, None  # Variables to track previous position

        for row in reader:
            counter += 1
            pos_x_new, pos_y_new = float(row[3]), float(row[4])  # Get the X and Y positions

            # Check if a fixation occurred by comparing the current position with the previous position
            if pos_x is not None and pos_y is not None:
                pos_diff = abs(pos_x - pos_x_new) + abs(pos_y - pos_y_new)
                if pos_diff > threshold:
                    nfix += 1  # Increment the NFix counter

            pos_x, pos_y = pos_x_new, pos_y_new  # Update the previous position
        if(ShowPercentage == True):
            return (nfix/counter)*100
        else:
            return nfix
def ConvergTime(data_file):
    gaze_times = []
    last_target = None
    last_gaze_time = None

    with open(data_file, 'r') as file:
        reader = csv.DictReader(file, delimiter=';')
        for row in reader:
            gaze_time = float(row['GazeTime'])
            target = row['Target']

            if last_target is None:
                last_target = target
                last_gaze_time = gaze_time
                continue

            if target != last_target:
                gaze_times.append(last_gaze_time)
                last_target = target
                last_gaze_time = gaze_time

        # Add the last recorded gaze time
        gaze_times.append(last_gaze_time)
        #print(gaze_times)
    if len(gaze_times) > 0:
        converg_time = sum(gaze_times) / len(gaze_times)
        return converg_time
    else:
        return 0.0  # No gaze data available


# Usage examples
PercFixInside()
print()
print()
nfix = NFix(dataset,0.3,True)
print("Nfix: "+ str(nfix))
print()
print()
converg_time = ConvergTime(dataset)
print("ConvergTime:", converg_time)

#nfix_value = NFix(dataset,0.3,True)
#print("NFix value:", nfix_value)
